﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace SharpGL
{
	// Kieu enum cho nut chon mau
	public enum ButtonColor
	{
		LEFT,
		RIGHT
	}

	// Kieu enum cho nut chon hinh ve
	public enum ShapeMode
	{
		LINE,
		CIRCLE,
		RECTANGLE,
		ELLIPSE,
		TRIANGLE,
		PENTAGON,
		HEXAGON,
		POLYGON,
		FLOOD_FILL
	}

	// Kiểu enum Menu cho checklistbox
	public enum Menu
	{
		DRAWING,
		TRANSLATE,
		ROTATE,
		SCALE
	}

	// Kieu MyBitMap de luu cac control points da ve
	public struct MyBitMap
	{
		public List<Point> controlPoints;
		public Color colorUse;
		public ShapeMode type;
		public int brushSize;
		// Phuong thuc khoi tao cho struct MyBitMap
		public MyBitMap(Color _color, ShapeMode _type, int size){
			controlPoints = new List<Point>(); // Khoi tao list
			// Gan cac thong so can thiet
			colorUse = _color;
			type = _type;
			brushSize = size;
		}
	}

	public partial class Form_Paint : Form
	{
		// Tọa độ điểm di chuyển sau khi chọn menu de thuc hien phep translate, rotate & scale
		Point menuStart, menuEnd;

		// Mac dinh check list box la drawing
		Menu chooseItem = SharpGL.Menu.DRAWING;

		// Biến kiểm tra chúng ta có pushMatrix hay không. Mac dinh la false
		bool isPushMatrix = false;

		Color colorUserColor; // Bien mau de ve hinh
		ShapeMode shShape; // 0 neu muon ve duong thang, 1 neu duong tron, ...

		Point pStart, pEnd; // Toa do diem dau va diem cuoi
							// Point thuoc lop System.Drawing
		Point pMid; // De ve da giac
		int isDown; // Bien kiem soat con tro chuot co dang duoc giu khong
		int currentSize; // Kich co ve hien tai

		ButtonColor currentButtonColor; // Nut chon mau hien tai

		List<MyBitMap> bm = new List<MyBitMap>(); // Dung de luu tru cac doi tuong da ve

		public Form_Paint()
		{
			InitializeComponent();
			colorUserColor = Color.White; // Gia tri mac dinh la mau trang
			currentButtonColor = ButtonColor.LEFT; // Mac dinh la nut ben trai
			shShape = ShapeMode.LINE; // Mac dinh ve duong thang
			cBox_Choose_Size.SelectedIndex = 0; // Mac dinh net ve hien thi la 1
			chkLstBox_Options.SetItemChecked(0, true); // Mặc đinh là tick vào ô Drawing 
													   // Khoi tao toa diem ban dau
			pStart = new Point(-1, -1);

			//// Cap phat vung nho cho Bitmap
			//bm = new Bitmap(this.Width, this.Height); // kich thuoc bitmap bang voi form1
			//gr = Graphics.FromImage(bm); // Truyen doi tuong Bitmap vao de ve
		}

		private void openGLControl1_Load(object sender, EventArgs e)
		{

		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void bt_Palette_Click(object sender, EventArgs e)
		{
			// Goi hop thoai chon mau
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				// Neu nguoi dung chon mau tai button trai thi cap nhat back color cho nut do
				// va nguoc lai
				if (currentButtonColor == ButtonColor.LEFT)
					bt_Left_Color.BackColor = colorDialog1.Color;
				else
					bt_Right_Color.BackColor = colorDialog1.Color;

				colorUserColor = colorDialog1.Color; // Luu lai mau user chon
			}
		}

		// Nguoi dung chon chuc nang ve duong thang
		private void bt_Line_Click(object sender, EventArgs e)
		{

			// Unchecked các menu còn lại
			for (int i = 0; i < 4; i++)
			{
				chkLstBox_Options.SetItemChecked(i, false);
			}

			// Check menu Drawing
			chkLstBox_Options.SetItemChecked(0, true);
			// Thiet lap che do la DRAWING
			chooseItem = SharpGL.Menu.DRAWING;

			shShape = ShapeMode.LINE; // Nguoi dung chon ve duong thang
		}

		// Nguoi dung chon chuc nang ve hinh chu nhat
		private void bt_Rec_Click(object sender, EventArgs e)
		{
			// Unchecked các menu còn lại
			for (int i = 0; i < 4; i++)
			{
				chkLstBox_Options.SetItemChecked(i, false);
			}

			// Check menu Drawing
			chkLstBox_Options.SetItemChecked(0, true);
			chooseItem = SharpGL.Menu.DRAWING;
			shShape = ShapeMode.RECTANGLE;
		}

		// Nguoi dung chon chuc nang ve tam giac deu
		private void bt_Triangle_Click(object sender, EventArgs e)
		{
			// Unchecked các menu còn lại
			for (int i = 0; i < 4; i++)
			{
				chkLstBox_Options.SetItemChecked(i, false);
			}
			// Check menu Drawing
			chkLstBox_Options.SetItemChecked(0, true);
			chooseItem = SharpGL.Menu.DRAWING;
			shShape = ShapeMode.TRIANGLE;
		}
		// Bat su kien nguoi dung ve ngu giac deu
		private void bt_Pentagon_Click(object sender, EventArgs e)
		{
			// Unchecked các menu còn lại
			for (int i = 0; i < 4; i++)
			{
				chkLstBox_Options.SetItemChecked(i, false);
			}
			// Check menu Drawing
			chkLstBox_Options.SetItemChecked(0, true);
			chooseItem = SharpGL.Menu.DRAWING;
			shShape = ShapeMode.PENTAGON;
		}

		// Bat su kien nguoi dung ve luc giac deu
		private void bt_Hexagon_Click(object sender, EventArgs e)
		{
			// Unchecked các menu còn lại
			for (int i = 0; i < 4; i++)
			{
				chkLstBox_Options.SetItemChecked(i, false);
			}
			// Check menu Drawing
			chkLstBox_Options.SetItemChecked(0, true);
			chooseItem = SharpGL.Menu.DRAWING;
			shShape = ShapeMode.HEXAGON;
		}

		// Bat su kien nguoi dung ve duong tron
		private void bt_Circle_Click(object sender, EventArgs e)
		{
			// Unchecked các menu còn lại
			for (int i = 0; i < 4; i++)
			{
				chkLstBox_Options.SetItemChecked(i, false);
			}
			// Check menu Drawing
			chkLstBox_Options.SetItemChecked(0, true);
			chooseItem = SharpGL.Menu.DRAWING;
			shShape = ShapeMode.CIRCLE;
		}

		// Bat su kien nguoi dung ve ellipse
		private void bt_Ellipse_Click(object sender, EventArgs e)
		{

			// Unchecked các menu còn lại
			for (int i = 0; i < 4; i++)
			{
				chkLstBox_Options.SetItemChecked(i, false);
			}
			// Check menu Drawing
			chkLstBox_Options.SetItemChecked(0, true);
			chooseItem = SharpGL.Menu.DRAWING;
			shShape = ShapeMode.ELLIPSE;
		}

		// Ham khoi tao cho opengl
		private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
		{
			// get the openGL object
			OpenGL gl = openGLControl.OpenGL;
			// set the clear color: dat mau nen
			// alpha: do trong suot
			gl.ClearColor(0, 0, 0, 0);
			// set the projection matrix
			// Xet ma tran phep chieu
			// 2D: chỉ quan tam projection matrix
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			// load the identify
			// Xét ma trận hiện hành là ma trận đơn vị
			gl.LoadIdentity();
		}

		// Ham ve doan thang
		private void drawLine(OpenGL gl)
		{
			gl.Enable(OpenGL.GL_LINE_SMOOTH); // Lam tron cac diem ve, cho duong thang muot hon
			gl.Begin(OpenGL.GL_LINES);
			gl.Vertex(pStart.X, gl.RenderContextProvider.Height - pStart.Y);
			gl.Vertex(pEnd.X, gl.RenderContextProvider.Height - pEnd.Y);
			gl.End();
			gl.Flush();
			gl.Disable(OpenGL.GL_LINE_SMOOTH);
		}
		// Overide ham ve doan thang co truyen diem p1, p2
		private void drawLine(OpenGL gl, Point p1, Point p2)
		{
			gl.Enable(OpenGL.GL_LINE_SMOOTH); // Lam tron cac diem ve, cho duong thang muot hon
			gl.Begin(OpenGL.GL_LINES);
			gl.Vertex(p1.X, gl.RenderContextProvider.Height - p1.Y);
			gl.Vertex(p2.X, gl.RenderContextProvider.Height - p2.Y);
			gl.End();
			gl.Flush();
			gl.Disable(OpenGL.GL_LINE_SMOOTH);
		}

		// Ham tinh khoang cach giua pStart va pEnd
		private void calculateDistance(Point a, Point b, out double d)
		{
			d = Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
		}

		private void calculateDistance(int xa, int ya, int xb, int yb, out double d)
		{
			d = Math.Sqrt(Math.Pow(xa - xb, 2) + Math.Pow(ya - yb, 2));
		}

		// Ham ve cac diem trong diem doi xung trong duong tron
		private void put8Pixel(OpenGL gl, int a, int b, int x, int y)
		{
			gl.PointSize(currentSize);
			gl.Begin(OpenGL.GL_POINTS);
			gl.Vertex(a + x, b + y);
			gl.Vertex(a + x, b - y);
			gl.Vertex(a - x, b + y);
			gl.Vertex(a - x, b - y);
			gl.Vertex(a + y, b + x);
			gl.Vertex(a - y, b + x);
			gl.Vertex(a + y, b - x);
			gl.Vertex(a - y, b - x);
			gl.End();
			gl.Flush();
		}

		// Ham ve hinh tron
		private void drawCircle(OpenGL gl)
		{
			#region Cach 1: Su dung luong giac
			/*
			// Y tuong: Ve duong tron bang cach chia duong tron thanh cac segments 
			// (segments cac lon thi duong cong se cang muot)
			// Ta se chay Goc alpha tu 0 - 360 độ va alpha += 360 / totalSegments cho mỗi lần duyệt
			// Lưu ý: goc alpha đươc tính theo radian nên cần phải đổi sang radian:
			//		alpha_rad = alpha * 2*PI / 360 = alpha * PI/180
			// Toa độ x, y mỗi lần duyệt bằng: x = r*cos(alpha_rad), y = r*sin(alpha_rad)

			const int totalSegments = 90; // số lượng các segments
			// Ban kinh la đoặn thẳng nối từ pStart đến pEnd
			double r;
			calculateDistance(out r);

			// Bat dau ve
			gl.Enable(OpenGL.GL_LINE_SMOOTH);
			gl.Begin(OpenGL.GL_LINE_LOOP);


			for (int alpha = 0; alpha < 360; alpha += 360 / totalSegments) {
				// Đổi về radian
				double alpha_rad = alpha * Math.PI / 180;
				// Tinh x, y
				gl.Vertex(pEnd.X + r * Math.Cos(alpha_rad), pEnd.Y + r * Math.Sin(alpha_rad));
			}

			gl.End();
			gl.Flush();
			gl.Disable(OpenGL.GL_LINE_SMOOTH);
			*/
			#endregion
			#region Cach 2: Su dung thuat toan MidPoint
			// Ban kinh la 1 nửa của đường chéo hình vuông, tức là 1 nửa của pStart và pEnd
			double r;
			calculateDistance(pStart, pEnd, out r);
			r /= 2;

			// Tam duong tron tai trung diem cua doan thang noi pStart và pEnd
			int xc = (pStart.X + pEnd.X) / 2;
			int yc = (pStart.Y + pEnd.Y) / 2;

			// Giả sử xét tâm tại 0
			int x = 0;
			int y = (int)r;
			int p = (int)(5 / 4 - r);

			// Ve diem  dau (0, r)
			put8Pixel(gl, xc, yc, x, y);

			while (x < y)
			{
				x++;
				if (p < 0)
					p += 2 * x + 3;
				else
				{
					y--;
					p += 2 * (x - y) + 5;
				}
				put8Pixel(gl, xc, yc, x, y);
			}
			#endregion
		}
		// Overide ham ve hinh tron co them 2 tham so diem: p1, p2
		private void drawCircle(OpenGL gl, Point p1, Point p2)
		{
			#region Cach 2: Su dung thuat toan MidPoint
			// Ban kinh la 1 nửa của đường chéo hình vuông, tức là 1 nửa của pStart và pEnd
			double r;
			calculateDistance(p1, p2, out r);
			r /= 2;

			// Tam duong tron tai trung diem cua doan thang noi pStart và pEnd
			int xc = (p1.X + p2.X) / 2;
			int yc = (p1.Y + p2.Y) / 2;

			// Giả sử xét tâm tại 0
			int x = 0;
			int y = (int)r;
			int p = (int)(5 / 4 - r);

			// Ve diem  dau (0, r)
			put8Pixel(gl, xc, yc, x, y);

			while (x < y)
			{
				x++;
				if (p < 0)
					p += 2 * x + 3;
				else
				{
					y--;
					p += 2 * (x - y) + 5;
				}
				put8Pixel(gl, xc, yc, x, y);
			}
			#endregion
		}

		// Ham ve cac diem doi xung trong ellipse
		private void put4Pixel(OpenGL gl, int a, int b, int x, int y)
		{
			gl.PointSize(currentSize);
			gl.Begin(OpenGL.GL_POINTS);
			gl.Vertex(a + x, b + y);
			gl.Vertex(a + x, b - y);
			gl.Vertex(a - x, b - y);
			gl.Vertex(a - x, b + y);
			gl.End();
			gl.Flush();
		}

		// Ham lam tron
		int Round(double x)
		{
			return (int)(x + 0.5);
		}

		// Ham ve ellipse bang thuat toan Midpoint
		private void drawEllipse(OpenGL gl)
		{
			#region Ve ellipse bang thuat toan Midpoint
			// Gia su ban dau xet tai tam 0(0, 0)

			// Tinh tam C(xc, yc) cua ellipse
			// Dat tam C la trung diem cua doan thang noi pStart va pEnd
			int xc = Round((double)(pStart.X + pEnd.X) / 2);
			int yc = Round((double)(pStart.Y + pEnd.Y) / 2);

			// Goi A(xa, ya) la giao diem cua 0x va ellipse
			int xa = pEnd.X;
			int ya = Round((double)(pStart.Y + pEnd.Y) / 2);

			// Goi B(xb, yb) la giao diem cua 0y va ellipse
			int xb = Round((double)(pStart.X + pEnd.X) / 2);
			int yb = pStart.Y;

			// Tinh rx va ry
			double rx;
			calculateDistance(xa, ya, xc, yc, out rx);
			double ry;
			calculateDistance(xb, yb, xc, yc, out ry);

			// Diem dau
			int x = 0;
			int y = Round(ry);

			double ry2 = ry * ry; // ry^2
			double rx2 = rx * rx; // rx^2

			double p = ry2 - rx2 * ry + (1 / 4) * rx2;
			double A = 2 * ry2 * x;
			double B = 2 * rx2 * y;

			// Ve 4 diem dau
			put4Pixel(gl, xc, yc, x, y);
			// Xét vùng 1: 0 < |dy/dx| <= 1
			int k = 0;
			while (A < B)
			{
				x++;
				if (p < 0)
				{
					A += 2 * ry2;
					p += A + ry2;
				}
				else
				{
					y--;
					A += 2 * ry2;
					B -= 2 * rx2;
					p += A - B + ry2;
				}
				put4Pixel(gl, xc, yc, x, y);
			}

			// Xét vùng 2: |dy/dx| > 1
			float xlast = x, ylast = y;
			A = 2 * ry2 * xlast;
			B = 2 * rx2 * ylast;
			p = ry2 * Math.Pow((xlast + 1 / 2), 2) + rx2 * Math.Pow((ylast - 1), 2) - rx2 * ry2;

			k = 0;
			while (y != 0)
			{
				y--;
				if (p < 0)
				{
					x++;
					A += 2 * ry2;
					B -= 2 * rx2;
					p += A - B + rx2;
				}
				else
				{
					B -= 2 * rx2;
					p += -B + rx2;
				}
				put4Pixel(gl, xc, yc, x, y);
			}

			#endregion
		}
		// Overide ham ve ellipse co them 2 tham so diem: p1, p2
		private void drawEllipse(OpenGL gl, Point p1, Point p2)
		{
			#region Ve ellipse bang thuat toan Midpoint
			// Gia su ban dau xet tai tam 0(0, 0)

			// Tinh tam C(xc, yc) cua ellipse
			// Dat tam C la trung diem cua doan thang noi pStart va pEnd
			int xc = Round((double)(p1.X + p2.X) / 2);
			int yc = Round((double)(p1.Y + p2.Y) / 2);

			// Goi A(xa, ya) la giao diem cua 0x va ellipse
			int xa = p2.X;
			int ya = Round((double)(p1.Y + p2.Y) / 2);

			// Goi B(xb, yb) la giao diem cua 0y va ellipse
			int xb = Round((double)(p1.X + p2.X) / 2);
			int yb = p1.Y;

			// Tinh rx va ry
			double rx;
			calculateDistance(xa, ya, xc, yc, out rx);
			double ry;
			calculateDistance(xb, yb, xc, yc, out ry);

			// Diem dau
			int x = 0;
			int y = Round(ry);

			double ry2 = ry * ry; // ry^2
			double rx2 = rx * rx; // rx^2

			double p = ry2 - rx2 * ry + (1 / 4) * rx2;
			double A = 2 * ry2 * x;
			double B = 2 * rx2 * y;

			// Ve 4 diem dau
			put4Pixel(gl, xc, yc, x, y);
			// Xét vùng 1: 0 < |dy/dx| <= 1
			int k = 0;
			while (A < B)
			{
				x++;
				if (p < 0)
				{
					A += 2 * ry2;
					p += A + ry2;
				}
				else
				{
					y--;
					A += 2 * ry2;
					B -= 2 * rx2;
					p += A - B + ry2;
				}
				put4Pixel(gl, xc, yc, x, y);
			}

			// Xét vùng 2: |dy/dx| > 1
			float xlast = x, ylast = y;
			A = 2 * ry2 * xlast;
			B = 2 * rx2 * ylast;
			p = ry2 * Math.Pow((xlast + 1 / 2), 2) + rx2 * Math.Pow((ylast - 1), 2) - rx2 * ry2;

			k = 0;
			while (y != 0)
			{
				y--;
				if (p < 0)
				{
					x++;
					A += 2 * ry2;
					B -= 2 * rx2;
					p += A - B + rx2;
				}
				else
				{
					B -= 2 * rx2;
					p += -B + rx2;
				}
				put4Pixel(gl, xc, yc, x, y);
			}

			#endregion
		}

		// Ham ve hinh chu nhat
		private void drawRec(OpenGL gl)
		{
			gl.Enable(OpenGL.GL_LINE_SMOOTH); // Lam tron cac diem ve, cho duong thang muot hon
			gl.Begin(OpenGL.GL_LINE_LOOP);
			// Toa do diem dau (x1, y1)
			// Toa do diem cuoi (x2, y2)
			gl.Vertex(pStart.X, gl.RenderContextProvider.Height - pStart.Y);
			// Toa do diem 2 (x2, y1)
			gl.Vertex(pEnd.X, gl.RenderContextProvider.Height - pStart.Y);
			// Toa do diem 3 (x2, y2)
			gl.Vertex(pEnd.X, gl.RenderContextProvider.Height - pEnd.Y);
			// Toa do diem 4 (x1, y2)
			gl.Vertex(pStart.X, gl.RenderContextProvider.Height - pEnd.Y);
			gl.End();
			gl.Flush();
			gl.Disable(OpenGL.GL_LINE_SMOOTH);
		}
		// Overide ham ve rectangle co them 2 tham so diem: p1, p2
		private void drawRec(OpenGL gl, Point p1, Point p2)
		{
			gl.Enable(OpenGL.GL_LINE_SMOOTH); // Lam tron cac diem ve, cho duong thang muot hon
			gl.Begin(OpenGL.GL_LINE_LOOP);
			// Toa do diem dau (x1, y1)
			// Toa do diem cuoi (x2, y2)
			gl.Vertex(p1.X, gl.RenderContextProvider.Height - p1.Y);
			// Toa do diem 2 (x2, y1)
			gl.Vertex(p2.X, gl.RenderContextProvider.Height - p1.Y);
			// Toa do diem 3 (x2, y2)
			gl.Vertex(p2.X, gl.RenderContextProvider.Height - p2.Y);
			// Toa do diem 4 (x1, y2)
			gl.Vertex(p1.X, gl.RenderContextProvider.Height - p2.Y);
			gl.End();
			gl.Flush();
			gl.Disable(OpenGL.GL_LINE_SMOOTH);
		}

		// Ham ve tam giac
		private void drawTriangle(OpenGL gl)
		{
			#region VeTamGiacCan
			/*
			gl.Enable(OpenGL.GL_LINE_SMOOTH); // Lam tron cac diem ve, cho duong thang muot hon
			gl.Begin(OpenGL.GL_LINE_LOOP); // Ve tam giac
			gl.Vertex(pStart.X, gl.RenderContextProvider.Height - pStart.Y); // Dinh A(x1, y1)
			gl.Vertex(pEnd.X, gl.RenderContextProvider.Height - pEnd.Y); // Dinh B(x2, y2)

			if (pEnd.X < pStart.X) // Neu nguoi dung keo chuot qua trai
				gl.Vertex(pStart.X + Math.Abs(pStart.X - pEnd.X), gl.RenderContextProvider.Height - pEnd.Y);
			else // Neu nguoi dung keo chuot qua phai
				gl.Vertex(pStart.X - Math.Abs(pStart.X - pEnd.X), gl.RenderContextProvider.Height - pEnd.Y);
																		// Dinh C(x1 - abs(x2 - x1), y2)
			gl.End(); // Kết thúc
			gl.Flush(); // Thuc hien ve ngay thay vi phai doi sau 1 thoi gian
						// Bản chất khi vẽ thì nó vẽ lên vùng nhớ Buffer
						// Do đó cần dùng hàm Flush để đẩy vùng nhớ Buffer này lên màn hình
			gl.Disable(OpenGL.GL_LINE_SMOOTH);
			*/
			#endregion
			#region Ve Tam giac deu bang pp quay diem
			// Ý tưởng: Các đỉnh của ngũ giác đều quay 1 goc alpha = 120*PI/180 độ (đổi về radian)
			// B1: Gán pStart là tâm
			// B2: Quay pEnd theo công thức
			//	x' = x*cos(alpha) - sin(alpha)*y
			//	y' = x*sin(alpha) + y*cos(alpha)
			const int totalSegments = 3; // số lượng các segments

			// Ban kinh bằng 1 nửa của đoạn thẳng pStart, pEnd
			double r;
			calculateDistance(pStart, pEnd, out r);
			r /= 2;

			// Tam duong tron tai trung diem cua doan thang noi pStart và pEnd
			int xc = (pStart.X + pEnd.X) / 2;
			int yc = (pStart.Y + pEnd.Y) / 2;

			int x = 0;
			int y = (int)r;

			// Bat dau ve
			gl.Enable(OpenGL.GL_LINE_SMOOTH);
			gl.Begin(OpenGL.GL_LINE_LOOP);

			for (int alpha = 0; alpha < 360; alpha += 360 / totalSegments)
			{
				// Đổi về radian
				double alpha_rad = alpha * Math.PI / 180;
				// Tinh x, y
				gl.Vertex(xc + x * Math.Cos(alpha_rad) - y * Math.Sin(alpha_rad)
					, yc + x * Math.Sin(alpha_rad) + y * Math.Cos(alpha_rad));
			}

			gl.End();
			gl.Flush();
			gl.Disable(OpenGL.GL_LINE_SMOOTH);
			#endregion
		}
		// Overide ham ve rectangle co them 2 tham so diem: p1, p2
		private void drawTriangle(OpenGL gl, Point p1, Point p2)
		{
			#region Ve Tam giac deu bang pp quay diem
			// Ý tưởng: Các đỉnh của ngũ giác đều quay 1 goc alpha = 120*PI/180 độ (đổi về radian)
			// B1: Gán pStart là tâm
			// B2: Quay pEnd theo công thức
			//	x' = x*cos(alpha) - sin(alpha)*y
			//	y' = x*sin(alpha) + y*cos(alpha)
			const int totalSegments = 3; // số lượng các segments

			// Ban kinh bằng 1 nửa của đoạn thẳng pStart, pEnd
			double r;
			calculateDistance(p1, p2, out r);
			r /= 2;

			// Tam duong tron tai trung diem cua doan thang noi pStart và pEnd
			int xc = (p1.X + p2.X) / 2;
			int yc = (p1.Y + p2.Y) / 2;

			int x = 0;
			int y = (int)r;

			// Bat dau ve
			gl.Enable(OpenGL.GL_LINE_SMOOTH);
			gl.Begin(OpenGL.GL_LINE_LOOP);

			for (int alpha = 0; alpha < 360; alpha += 360 / totalSegments)
			{
				// Đổi về radian
				double alpha_rad = alpha * Math.PI / 180;
				// Tinh x, y
				gl.Vertex(xc + x * Math.Cos(alpha_rad) - y * Math.Sin(alpha_rad)
					, yc + x * Math.Sin(alpha_rad) + y * Math.Cos(alpha_rad));
			}

			gl.End();
			gl.Flush();
			gl.Disable(OpenGL.GL_LINE_SMOOTH);
			#endregion
		}

		// Ham ve ngu giac deu
		private void drawPentagon(OpenGL gl)
		{
			#region Cach 1: Dung luong giac
			/*
			// Ý tưởng: ngũ giác đều chia đường tròn thành 5 đoạn. Mỗi đoạn cách nhau 72 độ
			// Làm tương tự như thuật toán vẽ đường tròn theo cách lượng giác
			const int totalSegments = 5; // số lượng các segments
			// Ban kinh la đoặn thẳng nối từ pStart đến pEnd
			double r;
			calculateDistance(out r);

			// Bat dau ve
			gl.Enable(OpenGL.GL_LINE_SMOOTH);
			gl.Begin(OpenGL.GL_LINE_LOOP);

			for (int alpha = 0; alpha < 360; alpha += 360 / totalSegments)
			{
				// Đổi về radian
				double alpha_rad = alpha * Math.PI / 180;
				// Tinh x, y
				gl.Vertex(pStart.X + r * Math.Cos(alpha_rad), pStart.Y + r * Math.Sin(alpha_rad));
			}

			gl.End();
			gl.Flush();
			gl.Disable(OpenGL.GL_LINE_SMOOTH);
			*/

			#endregion
			#region Cach 2: Dung phep quay diem
			// Ý tưởng: Các đỉnh của ngũ giác đều quay 1 goc alpha = 72*PI/180 độ (đổi về radian)
			// B1: Gán pStart là tâm
			// B2: Quay pEnd theo công thức
			//	x' = x*cos(alpha) - sin(alpha)*y
			//	y' = x*sin(alpha) + y*cos(alpha)
			const int totalSegments = 5; // số lượng các segments

			// Ban kinh bằng 1 nửa của đoạn thẳng pStart, pEnd
			double r;
			calculateDistance(pStart, pEnd, out r);
			r /= 2;

			// Tam duong tron tai trung diem cua doan thang noi pStart và pEnd
			int xc = (pStart.X + pEnd.X) / 2;
			int yc = (pStart.Y + pEnd.Y) / 2;

			int x = 0;
			int y = (int)r;

			// Bat dau ve
			gl.Enable(OpenGL.GL_LINE_SMOOTH);
			gl.Begin(OpenGL.GL_LINE_LOOP);

			for (int alpha = 0; alpha < 360; alpha += 360 / totalSegments)
			{
				// Đổi về radian
				double alpha_rad = alpha * Math.PI / 180;
				// Tinh x, y
				gl.Vertex(xc + x * Math.Cos(alpha_rad) - y * Math.Sin(alpha_rad)
					, yc + x * Math.Sin(alpha_rad) + y * Math.Cos(alpha_rad));
			}

			gl.End();
			gl.Flush();
			gl.Disable(OpenGL.GL_LINE_SMOOTH);
			#endregion
		}
		// Overide ham ve Pentagon co them 2 tham so diem: p1, p2
		private void drawPentagon(OpenGL gl, Point p1, Point p2)
		{
			#region Cach 2: Dung phep quay diem
			// Ý tưởng: Các đỉnh của ngũ giác đều quay 1 goc alpha = 72*PI/180 độ (đổi về radian)
			// B1: Gán pStart là tâm
			// B2: Quay pEnd theo công thức
			//	x' = x*cos(alpha) - sin(alpha)*y
			//	y' = x*sin(alpha) + y*cos(alpha)
			const int totalSegments = 5; // số lượng các segments

			// Ban kinh bằng 1 nửa của đoạn thẳng pStart, pEnd
			double r;
			calculateDistance(p1, p2, out r);
			r /= 2;

			// Tam duong tron tai trung diem cua doan thang noi pStart và pEnd
			int xc = (p1.X + p2.X) / 2;
			int yc = (p1.Y + p2.Y) / 2;

			int x = 0;
			int y = (int)r;

			// Bat dau ve
			gl.Enable(OpenGL.GL_LINE_SMOOTH);
			gl.Begin(OpenGL.GL_LINE_LOOP);

			for (int alpha = 0; alpha < 360; alpha += 360 / totalSegments)
			{
				// Đổi về radian
				double alpha_rad = alpha * Math.PI / 180;
				// Tinh x, y
				gl.Vertex(xc + x * Math.Cos(alpha_rad) - y * Math.Sin(alpha_rad)
					, yc + x * Math.Sin(alpha_rad) + y * Math.Cos(alpha_rad));
			}

			gl.End();
			gl.Flush();
			gl.Disable(OpenGL.GL_LINE_SMOOTH);
			#endregion
		}
		// Ham ve luc giac deu
		private void drawHexagon(OpenGL gl)
		{
			// Ý tưởng: Các đỉnh của ngũ giác đều quay 1 goc alpha = 60*PI/180 độ (đổi về radian)
			// B1: Gán pStart là tâm
			// B2: Quay pEnd theo công thức
			//	x' = x*cos(alpha) - sin(alpha)*y
			//	y' = x*sin(alpha) + y*cos(alpha)
			const int totalSegments = 6; // số lượng các segments
										 // Ban kinh bằng 1 nửa của đoạn thẳng đi qua pStart, pEnd
			double r;
			calculateDistance(pStart, pEnd, out r);
			r /= 2;

			// Tam duong tron tai trung diem cua doan thang noi pStart và pEnd
			int xc = (pStart.X + pEnd.X) / 2;
			int yc = (pStart.Y + pEnd.Y) / 2;

			int x = 0;
			int y = (int)r;

			// Bat dau ve
			gl.Enable(OpenGL.GL_LINE_SMOOTH);
			gl.Begin(OpenGL.GL_LINE_LOOP);

			for (int alpha = 0; alpha < 360; alpha += 360 / totalSegments)
			{
				// Đổi về radian
				double alpha_rad = alpha * Math.PI / 180;
				// Tinh x, y
				gl.Vertex(xc + x * Math.Cos(alpha_rad) - y * Math.Sin(alpha_rad)
					, yc + x * Math.Sin(alpha_rad) + y * Math.Cos(alpha_rad));
			}

			gl.End();
			gl.Flush();
			gl.Disable(OpenGL.GL_LINE_SMOOTH);
		}
		// Overide ham ve luc giac deu co them 2 tham so diem: p1, p2
		private void drawHexagon(OpenGL gl, Point p1, Point p2)
		{
			// Ý tưởng: Các đỉnh của ngũ giác đều quay 1 goc alpha = 60*PI/180 độ (đổi về radian)
			// B1: Gán pStart là tâm
			// B2: Quay pEnd theo công thức
			//	x' = x*cos(alpha) - sin(alpha)*y
			//	y' = x*sin(alpha) + y*cos(alpha)
			const int totalSegments = 6; // số lượng các segments
										 // Ban kinh bằng 1 nửa của đoạn thẳng đi qua pStart, pEnd
			double r;
			calculateDistance(p1, p2, out r);
			r /= 2;

			// Tam duong tron tai trung diem cua doan thang noi pStart và pEnd
			int xc = (p1.X + p2.X) / 2;
			int yc = (p1.Y + p2.Y) / 2;

			int x = 0;
			int y = (int)r;

			// Bat dau ve
			gl.Enable(OpenGL.GL_LINE_SMOOTH);
			gl.Begin(OpenGL.GL_LINE_LOOP);

			for (int alpha = 0; alpha < 360; alpha += 360 / totalSegments)
			{
				// Đổi về radian
				double alpha_rad = alpha * Math.PI / 180;
				// Tinh x, y
				gl.Vertex(xc + x * Math.Cos(alpha_rad) - y * Math.Sin(alpha_rad)
					, yc + x * Math.Sin(alpha_rad) + y * Math.Cos(alpha_rad));
			}

			gl.End();
			gl.Flush();
			gl.Disable(OpenGL.GL_LINE_SMOOTH);
		}

		// Ve da giac bat ky
		private void drawPolygon(OpenGL gl)
		{
			drawLine(gl);
		}
		// Overide ham ve luc giac deu co them 1 tham so: List<Point>
		private void drawPolygon(OpenGL gl, List<Point> lstPoints)
		{
			for (int i = 0; i < lstPoints.Count - 2; i += 2)
			{
				drawLine(gl, lstPoints[i], lstPoints[i+1]);
			}
		}

		// Ham translate
		private void translate(OpenGL gl)
		{
			gl.PushMatrix();
			// Tinh khoang doi trx va try
			int xTrans = menuEnd.X - menuStart.X;
			int yTrans = -menuEnd.Y + menuStart.Y;
			// Thoi hien translate
			gl.Translate(xTrans, yTrans, 0);

			isPushMatrix = true;
		}

		private void repaint(OpenGL gl)
		{
			for (int i = 0; i < bm.Count; i++)
			{
				Point p1 = new Point(bm[i].controlPoints[i].X, bm[i].controlPoints[i].Y);
				Point p2 = new Point(bm[i].controlPoints[i + 1].X, bm[i].controlPoints[i + 1].Y);
				// Chon mau
				gl.Color(bm[i].colorUse.R / 255.0, bm[i].colorUse.G / 255.0, bm[i].colorUse.B / 255.0, 0);
				// Thiet lap size cua net ve
				gl.LineWidth(bm[i].brushSize);

				switch (bm[i].type)
				{
					case ShapeMode.LINE:
						// Ve doan thang
						drawLine(gl, p1, p2);
						break;
					case ShapeMode.CIRCLE:
						// Ve duong tron
						drawCircle(gl, p1, p2);
						break;
					case ShapeMode.RECTANGLE:
						// Ve hinh chu nhat
						drawRec(gl, p1, p2);
						break;
					case ShapeMode.ELLIPSE:
						// Ve ellipse
						drawEllipse(gl, p1, p2);
						break;
					case ShapeMode.TRIANGLE:
						// Ve tam giac deu
						drawTriangle(gl, p1, p2);
						break;
					case ShapeMode.PENTAGON:
						// Ve ngu giac deu
						drawPentagon(gl, p1, p2);
						break;
					case ShapeMode.HEXAGON:
						// Ve luc giac deu
						drawHexagon(gl, p1, p2);
						break;
					case ShapeMode.POLYGON:
						// Ve da giac
						drawPolygon(gl, bm[i].controlPoints);
						break;
					case ShapeMode.FLOOD_FILL:
						// To mau bang thuat toan flood fill
						floodFill(p1.X, p1.Y);
						break;
				}
			}
		}

		private void openGLControl_OpenGLDraw(object sender, RenderEventArgs args)
		{
			if (isDown == 1) // Neu nguoi dung dang Mouse down thi moi ve
			{
				// get the OpenGL object
				OpenGL gl = openGLControl.OpenGL;
				// clear the color and depth buffer
				// Xóa màn hình
				gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

				// Thuc hien repaint
				repaint(gl);

				// Chon mau
				gl.Color(colorUserColor.R / 255.0, colorUserColor.G / 255.0, colorUserColor.B / 255.0, 0);
				// Thiet lap size cua net ve
				gl.LineWidth(currentSize);

				// Stopwatch ho tro do thoi gian
				Stopwatch myTimer = new Stopwatch();
				myTimer.Start(); // bat dau do

				//===================================================================//
				//===================================================================//
				//========================Nội dung của Lab 03========================//
				// Xét trường hợp menu được chọn
				// Với mỗi menu được chọn ta sẽ xét một ma trận khác
				// Do đó cần push ma trận model view vào stack
				if (chooseItem == SharpGL.Menu.TRANSLATE) // Dich chuyen
				{
					translate(gl);
				}
				else if (chooseItem == SharpGL.Menu.ROTATE) // Xoay
				{

				}
				else if (chooseItem == SharpGL.Menu.SCALE) // Zoom
				{

				}

				// Ve voi cho nay
				// ...
				switch (shShape)
				{
					case ShapeMode.LINE:
						// Ve doan thang
						drawLine(gl);
						break;
					case ShapeMode.CIRCLE:
						// Ve duong tron
						drawCircle(gl);
						break;
					case ShapeMode.RECTANGLE:
						// Ve hinh chu nhat
						drawRec(gl);
						break;
					case ShapeMode.ELLIPSE:
						// Ve ellipse
						drawEllipse(gl);
						break;
					case ShapeMode.TRIANGLE:
						// Ve tam giac deu
						drawTriangle(gl);
						break;
					case ShapeMode.PENTAGON:
						// Ve ngu giac deu
						drawPentagon(gl);
						break;
					case ShapeMode.HEXAGON:
						// Ve luc giac deu
						drawHexagon(gl);
						break;
					case ShapeMode.POLYGON:
						// Ve da giac
						drawPolygon(gl);
						break;
					case ShapeMode.FLOOD_FILL:
						// To mau bang thuat toan flood fill
						floodFill(pStart.X, pStart.Y);
						break;
				}

				
				myTimer.Stop(); // ket thuc do
				TimeSpan Time = myTimer.Elapsed; // Lay thoi gian troi qua
				tb_Time.Text = String.Format("{0} (sec)", Time.TotalSeconds); // In ra tb_Time

				// Kiem tra xem co push matrix vao stack khong?
				if (isPushMatrix == true)
				{
					gl.PopMatrix();
				}
			}
			else {
				if (pStart.X != -1)
				{
					// Thuc hien lui doi tuong da ve vao List<MyBitMap> bm
					MyBitMap tmp = new MyBitMap(colorUserColor, shShape, currentSize);
					tmp.controlPoints.Add(pStart);
					tmp.controlPoints.Add(pEnd);
					// Them tmp vao bm
					bm.Add(tmp);
				}
			}

		}


		private void openGLControl_Resized(object sender, EventArgs e)
		{
			// get the OpenGL object
			OpenGL gl = openGLControl.OpenGL;
			// set the projection matrix
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			// load the identify
			gl.LoadIdentity();
			// Create a perspective transformation
			gl.Viewport(0, 0, openGLControl.Width, openGLControl.Height); // Xét cái màn hình: Vẽ toàn bộ cái khung của OpenGL control

			// Hàm set up cái phép chiếu trực giao
			// Ở đây chính là cái size của khung OpenGL control
			gl.Ortho2D(0, openGLControl.Width, 0, openGLControl.Height);
		}

		// Ham getPixelColor
		private void getPixelColor(Point p, out Byte[] color)
		{
			OpenGL gl = openGLControl.OpenGL;
			color = new Byte[3];

			gl.ReadPixels(p.X, p.Y, 1, 1, OpenGL.GL_RGB, OpenGL.GL_FLOAT, color);
		}

		// Ham set pixel color
		private void setPixelColor(Point p)
		{
			OpenGL gl = openGLControl.OpenGL;
			// set mau
			gl.Color(colorUserColor.R / 255.0, colorUserColor.G / 255.0, colorUserColor.B / 255.0, 0);
			gl.Begin(OpenGL.GL_POINTS);
			gl.Vertex(p.X, p.Y);
			gl.End();
			gl.Flush();
		}

		private void floodFill(int x, int y)
		{
			Byte[] color;
			getPixelColor(pStart, out color);

			// Thuat toan to mau theo vet loang
			// Neu color cua pixel khac bien va chua to mau
			if (color[0] == 0 && color[1] == 0 && color[2] == 0)
			{
				setPixelColor(pStart);
				floodFill(x + 1, y);
				floodFill(x - 1, y);
				floodFill(x, y + 1);
				floodFill(x, y - 1);
			}
		}

		// Ham xu ly su kien to mau theo vet loang
		private void bt_Flood_Fill_Click(object sender, EventArgs e)
		{
			shShape = ShapeMode.FLOOD_FILL;
		}

		// Khi nguoi dung click button chon mau ben trai
		private void bt_Left_Color_Click(object sender, EventArgs e)
		{
			currentButtonColor = ButtonColor.LEFT;
			colorUserColor = bt_Left_Color.BackColor; // cap nhat mau hien tai
		}

		// Khi nguoi dung click button chon mau ben phai
		private void bt_Right_Color_Click(object sender, EventArgs e)
		{
			currentButtonColor = ButtonColor.RIGHT;
			colorUserColor = bt_Right_Color.BackColor; // Cap nhat mau  hien tai 
													   //khi nguoi dung click button phai
		}

		// Cap nhat diem cuoi khi nguoi dung dang keo chuot
		private void ctrl_OpenGLControl_MouseMove(object sender, MouseEventArgs e)
		{
			// Neu chuot dang di chuyen thi moi cap nhat diem pEnd
			if (isDown == 1)
			{
				// Xét menu đang chọn
				if (chooseItem == SharpGL.Menu.DRAWING)
				{
					// Cap nhat diem cuoi
					pEnd = new Point(e.Location.X, e.Location.Y);
				}
				else
				{
					// Cập nhật menuEnd
					menuEnd = new Point(e.Location.X, e.Location.Y);
				}

				// In toa do khi di chuyen chuot 
				lb_Coor.Text = e.X.ToString() + ", " + e.Y.ToString();
			}

		}

		// Cap nhat toa do diem cuoi khi nguoi dung buong chuot ra
		private void ctrl_OpenGLControl_MouseUp(object sender, MouseEventArgs e)
		{
			// Nếu chọn menu Drawing
			if (chooseItem != SharpGL.Menu.DRAWING)
			{
				// Cập nhật lại điểm menuEnd
				menuEnd = new Point(e.X, e.Y);
				isDown = 0;
			}
			else
			{
				// Neu nguoi dung khong ve da giac thi ket thuc viec ve hinh
				if (shShape != ShapeMode.POLYGON)
				{
					openGLControl.Cursor = Cursors.Default; // Tra ve con tro chuot nhu cu
					isDown = 0; // chuot het di chuyen

					//==================>>
					// Tạm thời không gán lại
					// mình sẽ tìm phương thức khác sau vì các thao tác chuyển dời ảnh cần các điểm này để vẽ lại

					// reset lai toa do
					//pStart = new Point(-1, -1);
					//pEnd = new Point(-1, -1);
				}

				//// Ve len bitmap
				//Pen pen = new Pen(colorUserColor);
				//gr.DrawLine(pen, pStart, pEnd);
				//this.BackgroundImage = (Bitmap)bm.Clone(); // Set lai background
			}

		}

		// Su kien chon size ve
		private void cBox_Choose_Size_SelectedIndexChanged(object sender, EventArgs e)
		{
			currentSize = int.Parse(cBox_Choose_Size.Text);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit(); // Tắt chuong trinh
		}

		private void bt_Polygon_Click(object sender, EventArgs e)
		{
			shShape = ShapeMode.POLYGON;
		}

		// Xu ly su kien nguoi dung click chuot
		private void openGLControl_MouseClick(object sender, MouseEventArgs e)
		{
			// Neu nguoi dung nhap chuot phai nghia la ket thuc ve da giac
			if (e.Button == MouseButtons.Right && shShape == ShapeMode.POLYGON)
			{
				isDown = 0;
			}
		}

		private void chkLstBox_Options_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (chkLstBox_Options.SelectedIndex)
			{
				case 0:
					chooseItem = SharpGL.Menu.DRAWING;
					break;
				case 1:
					chooseItem = SharpGL.Menu.TRANSLATE;
					break;
				case 2:
					chooseItem = SharpGL.Menu.ROTATE;
					break;
				case 3:
					chooseItem = SharpGL.Menu.SCALE;
					break;

			}

			for (int i = 0; i < 4; i++)
			{
				if (i != chkLstBox_Options.SelectedIndex)
				{
					chkLstBox_Options.SetItemChecked(i, false);
				}
			}
		}

		// Cap nhat diem dau khi nguoi dung bat dau giu chuot
		private void ctrl_OpenGLControl_MouseDown(object sender, MouseEventArgs e)
		{
			if (chooseItem == SharpGL.Menu.DRAWING)
			{
				if (shShape != ShapeMode.POLYGON)
				{
					// Cap nhat toa do diem dau
					pStart = new Point(e.Location.X, e.Location.Y); // e la tham so lien quan den su kien chon diem
					pEnd = new Point(e.X, e.Y); // Mac dinh pEnd = pStart
				}
				else
				{
					// Neu moi bat dau click
					if (pStart.X == -1)
					{
						pStart = new Point(e.X, e.Y);
						pEnd = new Point(e.X, e.Y); // Mac dinh pEnd = pStart
					}
					else // Nguoc lai
					{
						pStart = new Point(pEnd.X, pEnd.Y);
						pEnd = new Point(e.X, e.Y);
					}
				}
				
					
			}
			else
			{
				// Cap nhat toa do cho viec thuc hien cac phep transform
				menuStart = menuEnd = new Point(e.X, e.Y);
			}

			openGLControl.Cursor = Cursors.Cross; // Thay doi hinh dang con tro chuot khi ve
			isDown = 1; // Chuot dang bat dau di chuyen
		}

	}
}
