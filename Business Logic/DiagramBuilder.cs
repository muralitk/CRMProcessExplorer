using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRMProcessExplorer
{
    public class DiagramBuilder
    {
        private Color _fontColor = Color.Black;
        private int _boxWidth = 120;
        private int _boxHeight = 80; 
        private int _margin = 20;
        private int _horizontalSpace = 30;
        private int _verticalSpace = 30;
        private float _fontSize = 8.25f;
        private int imgWidth = 0;
        private int imgHeight = 0;
        private Graphics gr;
        private float _lineWidth = 2;
        private Color _lineColor = Color.Black;

        public Color _fgColor = Color.Black;
        public Color _bgColor = Color.White;
        public Color _workflowBoxFillColor = Color.FromArgb(119, 173, 230); //#77ADE6
        public Color _entityBoxFillColor = Color.FromArgb(199, 42, 27); // #C72A1B
        public Color _actionBoxFillColor = Color.FromArgb(247, 247, 110); // #F7F76E
        public Color _pluginBoxFillColor = Color.FromArgb(247, 173, 163); // #F7ADA3
        public Color _titleBoxFillColor = Color.LightGray;
        public Color _dialogBoxFillColor = Color.FromArgb(210, 217, 181); // #D2D9B5
        public Color _brBoxFillColor = Color.FromArgb(220, 110, 28); // #DC6E1C
        public Color _bpfBoxFillColor = Color.LightGreen;


        public bool ShowAssemblies { get; set; } = true;

        public MemoryStream PaintDiagram(ProcessDetailTN pdRoot, ref int width, ref int height, string startFromNodeID, ImageFormat imageType)
        {
            pdRoot = pdRoot.CloneMe();
            TreeView tv = new TreeView();
            tv.Nodes.Add(pdRoot);

            //reset image size
            imgHeight = 0;
            imgWidth = 0;
            MemoryStream Result = new MemoryStream();

            BuildTree(pdRoot, 0);

            Bitmap bmp = new Bitmap(imgWidth, imgHeight);
            gr = Graphics.FromImage(bmp);
            gr.Clear(_bgColor);
            DrawChart(pdRoot);

            Bitmap ResizedBMP = new Bitmap(bmp, new Size(imgWidth, imgHeight));
            ResizedBMP.Save(Result, imageType);
            ResizedBMP.Dispose();
            bmp.Dispose();
            gr.Dispose();

            width = imgWidth;
            height = imgHeight;

            return Result;
        }

        private void BuildTree(ProcessDetailTN oNode, int y)
        {
            if (!ShowAssemblies && oNode.Type == ProcessDetail.eTypes.PluginType)
                oNode.IsHidden = true;

            foreach (ProcessDetailTN childNode in oNode.Nodes)
            {
                BuildTree(childNode, y + 1);
            }

            //build node data
            //after checking for nodes we can add the current node
            int StartX;
            int StartY;
            int[] ResultsArr = new int[] {  GetXPosByOwnChildren(oNode),
                                            GetXPosByParentPreviousSibling(oNode),
                                            GetXPosByPreviousSibling(oNode),
                                            _margin };
            Array.Sort(ResultsArr);
            StartX = ResultsArr[3];
            StartY = (y * (_boxHeight + _verticalSpace)) + _margin;
            int width = _boxWidth;
            int height = _boxHeight;

            //update the coordinates of this box into the matrix, for later calculations
            oNode.X = StartX;
            oNode.Y = StartY;

            //update the image size
            if (imgWidth < (StartX + width + _margin))
                imgWidth = StartX + width + _margin;
            if (imgHeight < (StartY + height + _margin))
                imgHeight = StartY + height + _margin;
        }

        public Rectangle GetMainRectangle(ProcessDetailTN oNode)
        {
            int X = oNode.X;
            int Y = oNode.Y + 20;

            Rectangle Result = new Rectangle(X, Y, (int)(_boxWidth), (int)(_boxHeight) - 20);
            return Result;
        }


        public Rectangle GetTitleRectangle(ProcessDetailTN oNode)
        {
            int X = oNode.X;
            int Y = oNode.Y;

            Rectangle Result = new Rectangle(X, Y, (int)(_boxWidth), 20);
            return Result;
        }


        private void DrawChart(ProcessDetailTN oNode)
        {
            // Create font and brush.
            Font drawFont = new Font("Microsoft Sans Serif", _fontSize);
            SolidBrush drawBrush = new SolidBrush(_fontColor);
            Pen boxPen = new Pen(_lineColor, _lineWidth);
            var drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Center;

            //find children
            foreach (ProcessDetailTN childNode in oNode.Nodes)
            {
                if (!childNode.IsHidden)
                    DrawChart(childNode);
            }

            Rectangle outerRectangle = new Rectangle(oNode.X, oNode.Y, (int)(_boxWidth), (int)(_boxHeight));
            Rectangle titleRectangle = new Rectangle(oNode.X, oNode.Y, (int)(_boxWidth), 18);
            Rectangle detailRectangle = new Rectangle(oNode.X, oNode.Y + 20, (int)(_boxWidth), (int)(_boxHeight) - 20);

            gr.DrawRectangle(boxPen, titleRectangle);
            gr.DrawRectangle(boxPen, detailRectangle);

            Color detailFillColor = _titleBoxFillColor;
            Color titleFillColor = GetBoxColor(oNode); 

            gr.FillRectangle(new SolidBrush(detailFillColor), detailRectangle);
            gr.FillRectangle(new SolidBrush(titleFillColor), titleRectangle);

            // Draw string to screen.
            var displayName = string.Empty;
            var rectangleDisplayInfo = string.Empty;
            rectangleDisplayInfo = oNode.Text.Replace($"({oNode.Tag}) ", string.Empty);
            if (oNode.Type == ProcessDetail.eTypes.Entity)
            {
                displayName = oNode.Text;
                rectangleDisplayInfo = $"{rectangleDisplayInfo} ({oNode.PrimaryEntityName})";
            }
            else if (oNode.Type == ProcessDetail.eTypes.PluginType)
                displayName = oNode.Type.ToString();
            else
                displayName = oNode.Category.ToString();

            drawFormat.LineAlignment = StringAlignment.Center;
            drawFormat.Alignment = StringAlignment.Center;
            gr.DrawString(displayName, drawFont, drawBrush, titleRectangle, drawFormat);
            gr.DrawString(rectangleDisplayInfo, drawFont, drawBrush, detailRectangle, drawFormat);

            //draw connecting lines
            if (!oNode.IsRoot)
            {
                //all but the top box should have lines growing out of their top
                gr.DrawLine(boxPen, outerRectangle.Left + (_boxWidth / 2),
                                    outerRectangle.Top,
                                    outerRectangle.Left + (_boxWidth / 2),
                                    outerRectangle.Top - (_verticalSpace / 2));
            }
            if (oNode.Nodes.Count > 0)
            {
                //all nodes which have nodes should have lines coming from bottom down
                gr.DrawLine(boxPen, outerRectangle.Left + (_boxWidth / 2),
                                    outerRectangle.Top + _boxHeight,
                                    outerRectangle.Left + (_boxWidth / 2),
                                    outerRectangle.Top + _boxHeight + (_verticalSpace / 2));

            }
            if (!oNode.IsRoot && oNode.PrevNode != null)
            {
                //the prev node has the same parent node - connect to 2 nodes
                ProcessDetailTN prevNode = (ProcessDetailTN)oNode.PrevNode;
                Rectangle prevOuterRectangle = new Rectangle(prevNode.X, prevNode.Y, (int)(_boxWidth), (int)(_boxHeight));
                gr.DrawLine(boxPen, prevOuterRectangle.Left + (_boxWidth / 2) - (_lineWidth / 2),
                                    prevOuterRectangle.Top - (_verticalSpace / 2),
                                    outerRectangle.Left + (_boxWidth / 2) + (_lineWidth / 2),
                                    outerRectangle.Top - (_verticalSpace / 2));
            }
        }


        private int GetXPosByPreviousSibling(ProcessDetailTN CurrentNode)
        {
            int Result = -1;
            int X = -1;

            if (!CurrentNode.IsRoot)
            {
                ProcessDetailTN PrevSibling = (ProcessDetailTN)CurrentNode.PrevNode;

                if (PrevSibling != null)
                {
                    if (PrevSibling.Nodes.Count > 0)
                    {
                        X = Convert.ToInt32(GetMaxXOfDescendants((ProcessDetailTN)PrevSibling.LastNode));
                        Result = X + _boxWidth + _horizontalSpace;
                    }
                    else
                    {
                        Result = PrevSibling.X + _boxWidth + _horizontalSpace;
                    }
                }
            }

            return Result;
        }

        private int GetMaxXOfDescendants(ProcessDetailTN CurrentNode)
        {
            while (CurrentNode.Nodes.Count > 0)
            {
                CurrentNode = (ProcessDetailTN)CurrentNode.LastNode;
            }

            return CurrentNode.X;
        }

        private int GetXPosByOwnChildren(ProcessDetailTN CurrentNode)
        {
            int Result = -1;

            if (CurrentNode.Nodes.Count > 0)
            {
                int lastChildX = ((ProcessDetailTN)CurrentNode.LastNode).X;
                int firstChildX = ((ProcessDetailTN)CurrentNode.FirstNode).X;
                Result = (((lastChildX + _boxWidth) - firstChildX) / 2) - (_boxWidth / 2) + firstChildX;


            }
            return Result;
        }

        private int GetXPosByParentPreviousSibling(ProcessDetailTN CurrentNode)
        {
            int Result = -1;
            int X = -1;

            var Parent = (ProcessDetailTN)CurrentNode.Parent;
            if (!CurrentNode.IsRoot && Parent != null && !Parent.IsRoot)
            {
                var ParentPrevSibling = (ProcessDetailTN)Parent.PrevNode;
                if (ParentPrevSibling != null)
                {
                    if (ParentPrevSibling.Nodes.Count > 0)
                    {

                        X = GetMaxXOfDescendants((ProcessDetailTN)ParentPrevSibling.LastNode);
                        Result = X + _boxWidth + _horizontalSpace;
                    }
                    else
                    {

                        X = ParentPrevSibling.X;
                        Result = X + _boxWidth + _horizontalSpace;
                    }
                }
                else
                {

                    if (!CurrentNode.IsRoot && CurrentNode.Parent != null)
                    {
                        Result = GetXPosByParentPreviousSibling((ProcessDetailTN)CurrentNode.Parent);
                    }
                }
            }

            return Result;
        }

        private Color GetBoxColor(ProcessDetailTN node)
        {
            Color color = Color.White;

            if (node.Type == ProcessDetail.eTypes.Entity)
                color = _entityBoxFillColor;
            else if (node.Type == ProcessDetail.eTypes.Workflow)
            {
                switch (node.Category)
                {
                    case ProcessDetail.eCategories.Action:
                        color = _actionBoxFillColor;
                        break;
                    case ProcessDetail.eCategories.Workflow:
                        color = _workflowBoxFillColor;
                        break;
                    case ProcessDetail.eCategories.Dialog:
                        color = _dialogBoxFillColor;
                        break;
                    case ProcessDetail.eCategories.BusinessRule:
                        color = _brBoxFillColor;
                        break;
                    case ProcessDetail.eCategories.BusinessProcessFlow:
                        color = _bpfBoxFillColor;
                        break;
                }
            }
            else if (node.Type == ProcessDetail.eTypes.PluginType)
                color = _pluginBoxFillColor;

            return color;
        }
    }
}
