using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ex3__Calculate
{
    public partial class Calculate : Form
    {
        #region Static Flats 
        static string equation = ""; // Lưu trữ biểu thức thực hiện
        static bool IsOperandPressDynamic = false;  // Kiểm tra xem có vừa nhập toán tử không. Nếu có sẽ tạo sô mới khi nhấn tiếp theo
        static bool IsOperandPressStatic = false;
        static bool IsAffterEqual = false;  //Có tiếp tục sử dụng kết quả vừa tính để thực hiện tiếp các phép toán không.
        static bool isdoubleingPoint = false; // If you press '.', you can't again press
        static string recentPress = "";  // Lưu dấu phím vừa mới chọn. 
        #endregion
        public Calculate()
        {
            InitializeComponent();
            // Tại lỡ tạo một cái label nhưng không biết nó ẩn đâu mất, có thể làm mờ các component khác.
            this.Controls.Remove(equationLabel);
        }
        #region Format Equation
        public static void FormatExpression(ref string expression)
        {
            //expression = expression.Replace(" ", "");
            //expression = Regex.Replace(expression, @"\+|\-|\*|\/|\%|\^|\)|\(", delegate (Match match)
            //{
            //    return " " + match.Value + " ";
            //});
            expression = expression.Replace("  ", " ");
            expression = expression.Trim();
        }
        #endregion

        #region Event Click
        // Number Click
        private void button_Click(object sender, EventArgs e)
        {
            //Nếu vừa nhấn phím "=", trả mọi thứ lại ban đầu.
            if (IsAffterEqual && IsOperandPressDynamic == false)
            {
                // IsAffterEqual = false;
                // equation = "";
                this.Resest();

            }
            // Nếu như vừa nhấn phép toán.
            if (IsOperandPressDynamic)
            {
                resultTextBox.Text = "0";
                IsOperandPressDynamic = !IsOperandPressDynamic;
            }
            // Trường hợp nhập số vào lần đầu tiên, cần xóa số 0 ở đầu.
            if (resultTextBox.Text == "0")
                resultTextBox.Text = "";
            if (recentPress == ")")
            {
                equation += " * ";
                equationlb.Text = equation;
                resultTextBox.Text = "";
            }
            //Lấy thông tin của phím nhấn.
            Button b = (Button)sender;
            resultTextBox.Text += b.Text;
            recentPress = b.Text;
            label1.Focus(); // Bỏ chọn các button để tránh sai lệch các phím khi sử dụng
        }

        // Operand + - * / %
        private void OperationBtn_Click(object sender, EventArgs e)
        {
            // Hạng chế lỗi nhập nhiều toán tử của người dùng.
            if (IsOperandPressDynamic)
                return;


            Button b = (Button)sender;

            // Nếu vừa nhập ( or ) thì không cần lấy số hiện tại ở ô result
            if (recentPress == ")")
            {
                //  equation += " * ";

            }
            else
            //Trong trường hợp bt, sau khi ấn toán tử, số ở ô result sẽ được cập nhật vào equation
                if (!IsAffterEqual)
            {
                equation += resultTextBox.Text;
                //   IsAffterEqual = false;
            }
            //Th sau khi ấn phím bằng, nhấn toán tử là để tiếp tục tính trên kết quả vừa nhận được.
            else
            {
                equation = resultTextBox.Text;
                IsAffterEqual = false;
            }
            // Nhận toán tử vừa chọn.
            equation += " " + b.Text + " ";

            // Cập nhật
            recentPress = b.Text; // Lưu dấu phím vừa nhập
            equationlb.Text = equation;
            IsOperandPressDynamic = true;
            IsOperandPressStatic = true;
            label1.Focus(); // Bỏ chọn các button gay sai lệch khi sử dụng.
        }
        // Equal Button
        private void equlBtn_Click(object sender, EventArgs e)
        {
            //Đã có bấm ít nhất là một toán tử (phép tính)
            if (IsOperandPressStatic)
            {
                if (recentPress != ")")
                    //  Nhận vào toán hàng cuối trong phép toán, cập nhật equation Label.
                    equation += resultTextBox.Text;
                equationlb.Text = equation + "=";

                // Tính toán biểu thức.
                double result = EvaluatePostfix(InfixToPostFix(equation));
                // Th normal
                if (result != double.MaxValue)
                {
                    resultTextBox.Text = Convert.ToString(result);
                }
                else // Người dùng nhập sai
                {
                    resultTextBox.Text = "Error";
                    equation = "";
                }
            }
            else // Chính nó thoi, nhưng do cơ chế dấu bằng ở trên khi nhấn sẽ lại cộng chuỗi số ở resultTextBox vào.
            {
                equationlb.Text = resultTextBox.Text;
                equation = equationlb.Text;
            }

            // Cập nhật.
            IsAffterEqual = true;
            IsOperandPressStatic = false;
            IsOperandPressDynamic = false;
            recentPress = "=";
            label1.Focus(); // Bỏ chọn các button gay sai lệch khi sử dụng.
        }

        #region Bracket
        // Event of Bracket Left
        private void Bracket_Click(object sender, EventArgs e)
        {
            if (IsAffterEqual == true)
                Resest();


            // Thêm "(" và cập nhật.
            Button b = sender as Button;
            equation += " " + b.Text + " ";
            equationlb.Text = equation;
            recentPress = b.Text;
            //   IsOperandPress = true;
            label1.Focus(); // Bỏ chọn các button gay sai lệch khi sử dụng.

        }
        // Event of Bracket Right
        // Khi nhấn vào dấu ngoặc đóng, số đang được ở ô result phải được cập nhật vào biểu thức.
        private void BracketRight_Click(object sender, EventArgs e)
        {
            if (IsAffterEqual)
            {
                Resest();
            }
            else
            {
                // The diffirent
                equation += resultTextBox.Text;
                Button b = sender as Button;
                equation += " " + b.Text + " ";
                equationlb.Text = equation;
                recentPress = b.Text;
                // IsOperandPress = true;
                label1.Focus(); // Bỏ chọn các button gay sai lệch khi sử dụng. 
            }

        }

        #endregion
        // Floating Point
        #region Delete   
        private void CE_Button_Click(object sender, EventArgs e)
        {

            if (resultTextBox.Text.Length > 1)
            {
                resultTextBox.Text = resultTextBox.Text.Substring(0, resultTextBox.Text.Length - 1);
            }
            else
            {
                resultTextBox.Text = "0";
            }
            label1.Focus(); // Bỏ chọn các button gay sai lệch khi sử dụng.
        }
            private void ClearAll_Button_Click(object sender, EventArgs e)
        {
            Resest();
            label1.Focus(); // Bỏ chọn các button gay sai lệch khi sử dụng.
        }
        #endregion
        private void doubleingPoint_Click(object sender, EventArgs e)
        {
            // Số thực chỉ cần 1 dấu chấm là quá đủ.
            if (!isdoubleingPoint)
            {
                isdoubleingPoint = true;
                if (IsAffterEqual)
                {
                    equation = "";
                    IsOperandPressDynamic = false;
                    IsAffterEqual = false;
                    equationlb.Text = "";
                    recentPress = "";

                }
                if (resultTextBox.Text == "")
                    resultTextBox.Text += "0";
                resultTextBox.Text += ".";
            }
            label1.Focus(); // Bỏ chọn các button gay sai lệch khi sử dụng.
        }
        // +-- âm dương
        private void SignButton_Click(object sender, EventArgs e)
        {
            if (resultTextBox.Text[0] == '0')
                resultTextBox.Text = "-";

            else if (resultTextBox.Text[0] != '-')
                resultTextBox.Text = "-" + resultTextBox.Text;
            else
            {
                resultTextBox.Text = resultTextBox.Text.Substring(1);
            }
            //  recentPress = "-";
            if (IsAffterEqual)
            {

                IsAffterEqual = false;
                isdoubleingPoint = false;
                equationlb.Text = "";
                recentPress = "";
                equation = resultTextBox.Text;
            }
            label1.Focus(); // Bỏ chọn các button gay sai lệch khi sử dụng.
        }

        private void Persent_Click(object sender, EventArgs e)
        {
            MessageBox.Show("I do  not know the property of this symbol when I check the calculate in my PC " +
                "\n Please press another Button -_- !! ", "Sorry",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            label1.Focus(); // Bỏ chọn các button gay sai lệch khi sử dụng.
        }


        private void squareRoot_Click(object sender, EventArgs e)
        {
            double square = Convert.ToDouble(resultTextBox.Text);
            equation = resultTextBox.Text;
            equationlb.Text = "Square(" + square + ") = ";

            if (square >= 0)
            {
                square = (Math.Sqrt(square));
                resultTextBox.Text = Convert.ToString(square);
            }
            else
            {
                resultTextBox.Text = "Error";
            }

            IsAffterEqual = true;
            IsOperandPressDynamic = false;
           label1.Focus(); // Bỏ chọn các button gay sai lệch khi sử dụng.
        }

        private void inverseButton_Click(object sender, EventArgs e)
        {
            try
            {
                double inverse = Convert.ToDouble(resultTextBox.Text);
                equation = resultTextBox.Text;
                equationlb.Text = "Inverse(" + inverse + ") = ";


                if (inverse != 0)
                    resultTextBox.Text = Convert.ToString(1 / inverse);
                else
                {
                    resultTextBox.Text = "Cann't divide by Zero";
                }

                IsAffterEqual = true;
                IsOperandPressDynamic = false;
                label1.Focus(); // Bỏ chọn các button gay sai lệch khi sử dụng.
            }
            catch
            {
                Resest();
                return;
            }

        }

        #endregion

        #region Xử lí
        // Chuyển biểu thức trung tố sang hậu tố để tính toán dễ dàng
        public static string InfixToPostFix(string infix)
        {
            // Biểu thức có khi không đúng
            try
            {
                // Định dạng lại kiểu của biểu thức, nhiều trường hợp thừa " "
                FormatExpression(ref infix);
                //Chia biểu thức thành từng thành phần, dựa vào dấu " "
                IEnumerable<string> str = infix.Split(' ');
                // Tạo ngăn xếp cho thuật toán
                Stack<String> stack = new Stack<string>();
                StringBuilder postfix = new StringBuilder();

                // Thuật toán 
                // Nếu là số bỏ vào Stack
                // Nếu là "(" bỏ stack
                // Nếu là dấu ")" lấy các toán tử ra và cho vào Output cho
                //đến khi gặp dấu "(", dấu "(" cũng phải được đưa ra khỏi stack.
                // Nếu là toán tử,chừng nào toán tử ở đỉnh stack là toán tử có độ ưu tiên
                //lớn hơn thì còn lấy ra khỏi stack và cho vào Output. Đứa toán tử hiện tại vào Stack.
                // Nếu khi đến hết biểu thức, vẫn còn toán tử trong stack thì lấy ra và cho vào Output.

                foreach (string s in str)
                {
                    if (IsOperand(s))
                    {
                        postfix.Append(s).Append(" ");
                    }
                    else if (s == "(")
                    {
                        stack.Push(s);
                    }
                    else if (s == ")")
                    {
                        string x = stack.Pop();
                        while (x != "(")
                        {
                            postfix.Append(x).Append(" ");
                            x = stack.Pop();
                        }
                    }
                    else //IsOperand(s)
                    {
                        while (stack.Count > 0 && GetPriority(s) <= GetPriority(stack.Peek()))
                            postfix.Append(stack.Pop()).Append(" ");
                        stack.Push(s);
                    }
                }
                while (stack.Count > 0)
                    postfix.Append(stack.Pop()).Append(" ");

                return postfix.ToString();
            }
            catch
            {
                return "False"; // Chuổi nhận vào bị lỗi.
            }

        }


        // Calculate equation
        public static double EvaluatePostfix(string Postfix)
        {
            try
            {
                if (Postfix == "False") return double.MaxValue; // Chuỗi nhận được bị lỗi.
                                                                // Stack lưu các số khi thực hiện
                Stack<double> stack = new Stack<double>();

                // Tách các thành phần riêng biệt ra.
                Postfix = Postfix.Trim();
                IEnumerable<string> enumer = Postfix.Split(' ');

                // Thuật toán
                // Nếu gặp số thì cho vào Stack
                // Nếu gặp toán tử thì lấy 2 số ở đỉnh Stack và thực hiện phép toán, kết quả lưu ở biến y.
                foreach (string s in enumer)
                {
                    if (IsOperand(s))
                        stack.Push(double.Parse(s));

                    else
                    {
                        double x = stack.Pop();
                        double y = stack.Pop();

                        switch (s)
                        {
                            case "+": y += x; break;
                            case "-": y -= x; break;
                            case "*": y *= x; break;
                            case "/": y /= x; break;
                            case "%": y %= x; break;
                        }

                        stack.Push(y);
                    }
                }

                return stack.Pop();
            }
            catch
            {
                return double.MaxValue;
            }
        }

        // Khỏi tạo các giá trị như lúc đầu.
        public void Resest()
        {
            resultTextBox.Text = "0";
            equation = "";
            IsOperandPressDynamic = false;
            IsAffterEqual = false;
            isdoubleingPoint = false;
            equationlb.Text = "";
            resultTextBox.Text = "0";
            recentPress = "";
        }
        #endregion

        #region Kiểm tra, hỗ trợ
        // Xác định độ ưu tiên của toán tử.
        private static int GetPriority(string s)
        {
            if (s == "*" || s == "/" || s == "%")
                return 2;
            if (s == "+" || s == "-")
                return 1;

            return 0;
        }
        // Kiểm tra số thực.
        public static bool IsOperand(string str)
        {
            if (str.Length > 1)
            {
                if (str[0] == '-' || str.Contains(".")) return true;
            }
            return Regex.Match(str, @"^\d+$|^([a-z]|[A-Z])$").Success;
        }

        #endregion

        #region ToolBar
        private void aboutDeveloperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("My name Thao, Le Thi Phuong Thao \nMSSV: 18110366" +
                "\nVersion 1.1" +
                "\nUpdate 13-02-2019", "About Developer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void guideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Thông tin sản phẩm\n" +
                "Ở phiên bản này, bạn có thể: \n" +
                "Một số nút MC, M+, M-, đã bị loại bỏ do sự không cần thiết, bạn có thể sử dụng '(' hoặc ')' để tính bất kì phép tính mà bạn muốn.\n" +
                "Bạn có thể sử dụng bàn phím để nhập liệu.\n" +
                "Nếu bạn nhập sai, nội dung sẽ bị xóa", "Thông tin(Information)", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Calculate_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar.ToString())
            {
                case "1":
                    button1.PerformClick();
                    break;
                case "2":
                    button2.PerformClick();
                    break;
                case "3":
                    button3.PerformClick();
                    break;
                case "4":
                    button4.PerformClick();
                    break;
                case "5":
                    button5.PerformClick();
                    break;
                case "6":
                    button6.PerformClick();
                    break;
                case "7":
                    button7.PerformClick();
                    break;
                case "8":
                    button8.PerformClick();
                    break;
                case "9":
                    button9.PerformClick();
                    break;
                case "0":
                    button0.PerformClick();
                    break;
                case "+":
                    PlusBtn.PerformClick();
                    break;
                case "-":
                    SubtractBtn.PerformClick();
                    break;
                case "*":
                    MultipleBtn.PerformClick();
                    break;
                case "/":
                    DevideBtn.PerformClick();
                    break;
                case "=":
                    EqualBtn.PerformClick();
                    break;
                case "\r":
                    EqualBtn.PerformClick();
                    break;
                case " ":
                    EqualBtn.PerformClick();
                    break;
                case "(":
                    LeftBracketBtn.PerformClick();
                    break;
                case ")":
                    RightBracketBtn.PerformClick();
                    break;
                case ".":
                    PointBtn.PerformClick();
                    break;
                case "%":
                    Percent.PerformClick();
                    break;
                case "\b":
                    CEBtn.PerformClick();
                    break;

                default: break;
            }
        }
        #endregion

        #region Draft
        private void Calculate_Load(object sender, EventArgs e)
        {

        }
        private void menuStrip1_KeyDown(object sender, KeyEventArgs e)
        {
        }



        private void NumberKeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void Calculate_KeyUp(object sender, KeyEventArgs e)
        {
        }
        #endregion

    }
}
