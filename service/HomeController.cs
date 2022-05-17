using core.start.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace core.start.service
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        private string display = "0";
        private int displayFontSize = 28;

        private List<string> mrList = new List<string>();
        public static List<string> calcList = new List<string>();

        private static IFormFile? jsonFile;
        private static string location = "C:/Users/Public/Documents/calc.json";

        public static string[] resultStrings = new string[0];

        /// <summary>
        /// Объект калькулятора
        /// </summary>

        public static Calculator calc = new Calculator();

        /// <summary>
        /// Массив с калькуляторами для хранения объектов с различными состояниями
        /// </summary>
        public static Calculator[] calcs = new Calculator[0];


        protected void Page_Load(object sender, EventArgs e)
        {
            //File.WriteAllText(location, resultStrings);
        }



        public void Nubmer_Click(string s)
        {
            inputVal(s[0]);
        }

        /// <summary>
        /// Ввод символа
        /// </summary>
        /// <param name="c">Вводимый символ</param>
        public void inputVal(char c)
        {
            display = calc.inputValues(c);
        }

        protected void btn_Zero_Click(object sender, EventArgs e)
        {
            if (calc.arg == "") typeZeroComa();
            else display = calc.inputValues('0');
        }

        protected void btn_clear_Click(object sender, EventArgs e)
        {
            Array.Clear(calc.args, 0, 1);
            display = "0";
        }

        protected void btn_bspace_Click(object sender, EventArgs e)
        {
            display = calc.deleteSymbol();
        }



        protected void btn_Coma_Click(object sender, EventArgs e)
        {
            if (calc.arg.Contains(',') == false)
            {
                if (calc.arg == "")
                {
                    typeZeroComa();
                }
                else display = calc.inputValues(',');
            }
        }

        /// <summary>
        /// Ввод "0,"
        /// </summary>
        public void typeZeroComa()
        {
            display = calc.inputValues('0');
            display = calc.inputValues(',');
        }



        /// <summary>
        /// Нажатие одной из функций
        /// </summary>
        /// <param name = "f" > Делегат функции выполнения</param>
        /// <param name = "isExtraFunc" > Флаг функции одного аргумента</param>
        private void btn_Func_Click(Calculator.funcDeleg f, bool isExtraFunc)
        {
            if (isExtraFunc == true)
            {
                calc.extraFunc(f);
                display = calc.disp;
                saveStatus();
                //setTextSize();
            }
            else funcClick(f);
        }





        private void btn_Fraction_Click(object sender, EventArgs e)
        {
            calc.symbol = "1/";
            calc.fDeleg = new Calculator.CalcFunction().fraction;
            btn_Func_Click(calc.fDeleg, true);
        }

        /// <summary>
        /// Выполнение заданной функции
        /// </summary>
        /// <param name="f">Делегат заданной функции</param>
        private void funcClick(Calculator.funcDeleg f)
        {
            calc.resBtnFlag = false;

            switch (calc.index)
            {
                case false:
                    {
                        calc.fDeleg = f;
                        calc.tryToGetArg(calc.arg);
                        calc.funcFlag = true;
                        break;
                    }
                case true:
                    {
                        calc.tryToGetArg(calc.arg); ;
                        getResult(calc.fDeleg);
                        calc.funcFlag = true;
                        calc.fDeleg = f;
                        break;
                    }
            }

            display = calc.disp;
            calc.minus = false;
        }

        private void btn_click(string s, Calculator.funcDeleg f, bool isExtraFunc)
        {
            calc.symbol = s;

            if (calc.funcFlag == true)
            {
                calc.fDeleg = f;
                calc.resBtnFlag = false;
            }
            else
            {
                btn_Func_Click(f, isExtraFunc);
            }
        }

        public void setMR(int indexOf, int negative)
        {
            try
            {
                calc.mr[indexOf] += Convert.ToDouble(calc.arg) * negative;
            }
            catch
            {
                calc.mr[indexOf] = calc.args[0];
                calc.arg = calc.mr[indexOf].ToString();
            }

            setMrList(indexOf);

            calc.funcFlag = true;
            calc.resBtnFlag = true;

            //btn_MList.Enabled = true;

            calc.mrFlag = true;
        }

        public void setMrList(int indexOf)
        {

            try
            {
                //listBox_MR.Items[calc.mr.Length - 1].Text = calc.mr[indexOf].ToString();
                mrList[calc.mr.Length - 1] = calc.mr[indexOf].ToString();

            }
            catch
            {
                //listBox_MR.Items.Add(calc.mr[indexOf].ToString());
                mrList.Add(calc.mr[indexOf].ToString());
            }
        }

        //public void switchMRButtons()
        //{
        //    btn_MR.Enabled = false;
        //    btn_MC.Enabled = false;
        //}

        /// <summary>
        /// Получение аргумента из памяти
        /// </summary>
        /// <param name="indexOf">индекс аргумента в массиве памяти</param>
        public void getFromMR(int indexOf)
        {
            calc.arg = calc.mr[indexOf - 1].ToString();
            if (calc.mr[indexOf - 1] < 0) calc.minus = true;
            calc.disp = calc.displayOut(calc.arg);

            display = calc.disp;
        }

        /// <summary>
        /// Инициатор сохранения статуса калькулятора
        /// </summary>
        public void saveMe()
        {
            addCalc();
            try
            {
                SaverLoader.saveCalc(calc);
            }
            catch { }
        }

        /// <summary>
        /// Загрузка калькулятора
        /// </summary>
        /// <param name="i">Индекс загружаемого калькулятора из массива калькуляторов></param>
        public void loadMe(int i)
        {
            calc = new Calculator(calcs[i]);
            calc.resultString = "";

            display = calc.displayOut(calc.disp);

            resetCalc();

            calc.arg = calc.args[0].ToString();
        }

        /// <summary>
        /// Инициатор сброса калькулятора
        /// </summary>
        public void resetCalc()
        {
            calc.ResetCalc();
            displayFontSize = 26;
        }

        /// <summary>
        /// Пополнение массива состояний новым состоянием калькулятора
        /// </summary>
        private void addCalc()
        {
            int i = calcs.Length;
            Array.Resize(ref calcs, i + 1);
            calcs[calcs.Length - 1] = new Calculator(calc);
        }

        /// <summary>
        /// Добавление в список вычисления целиком
        /// </summary>
        /// <param name="c">Калькулятор</param>
        private void addToCalcList(Calculator c)
        {
            calcList.Add(c.resultString);
        }

        /// <summary>
        /// Инициатор получения результата
        /// </summary>
        /// <param name="cf">Делегат функции</param>
        private void getResult(Calculator.funcDeleg cf)
        {
            calc.getResult(cf);
            saveStatus();
        }

        /// <summary>
        /// Фиксация этапа вычисления
        /// </summary>
        private void saveStatus()
        {
            saveMe();
            if (calc.resultString != "") addToCalcList(calc);
            calc.resultString = "";
        }

        private void Combobox1_SelectionChanged(object sender, EventArgs e)
        {
            loadMe(calcList.IndexOf(calc.timeStampLabel));
        }

        //protected void History_Click(object sender, EventArgs e)
        //{
        //    Server.Transfer("HistoryForm.aspx", true);
        //}

        protected void btn_History_Click1(object sender, EventArgs e)
        {
            SaverLoader.btn_History_Click(location, calc.resultString);
        }

        protected void btn_Plus_Click(object sender, EventArgs e)
        {
            btn_click("+", new Calculator.CalcFunction().Summ, false);
        }

        protected void btn_Minus_Click1(object sender, EventArgs e)
        {
            btn_click("-", new Calculator.CalcFunction().differens, false);
        }

        protected void btn_Multiply_Click1(object sender, EventArgs e)
        {
            btn_click("×", new Calculator.CalcFunction().multiply, false);
        }

        protected void btn_Divide_Click1(object sender, EventArgs e)
        {
            btn_click("÷", new Calculator.CalcFunction().divide, false);
        }

        protected void btn_Result_Click1(object sender, EventArgs e)
        {
            try
            {
                calc.tryToGetArg(calc.arg);

                calc.resBtnFlag = true;
                calc.funcFlag = true;

                calc.getResult(calc.fDeleg);

                //label1.Text = calc.disp;

                saveStatus();
            }
            catch { }
        }

        protected void btn_MC_Click(object sender, EventArgs e)
        {
            calc.mr = new double[1];
            mrList.Clear();
            //btn_MList.Enabled = false;
            //listBox_MR.Items.Clear();
            //switchMRButtons();
        }

        protected void btn_MR_Click(object sender, EventArgs e)
        {
            getFromMR(calc.mr.Length - 1);
            calc.disp = calc.arg;

            calc.mrFlag = true;
        }

        protected void btn_MPlus_Click(object sender, EventArgs e)
        {
            setMR(calc.mr.Length - 1, 1);

            calc.mrFlag = true;

            //btn_MC.Enabled = true;
            //btn_MR.Enabled = true;
        }

        protected void btn_MMinus_Click(object sender, EventArgs e)
        {
            setMR(calc.mr.Length - 1, -1);
            calc.mrFlag = true;

            //btn_MC.Enabled = true;
            //btn_MR.Enabled = true;
        }

        protected void btn_MS_Click(object sender, EventArgs e)
        {
            int l = calc.mr.Length - 1;

            if (calc.mr.Length > 0)
            {
                Array.Resize(ref calc.mr, l + 2);
                l++;
            }
            setMR(l, 1);

            calc.mrFlag = true;

            //btn_MC.Enabled = true;
            //btn_MR.Enabled = true;
        }

        protected void btn_MList_Click(object sender, EventArgs e)
        {
            //MRPannel.Visible = true;
            //CalcPannel.Enabled = false;
        }

        protected void btn_Percent_Click1(object sender, EventArgs e)
        {
            calc.arg = "";
            calc.disp = "0";
            //label1.Text = calc.disp;
            //label1.Font.Size = 50;
        }

        protected void btn_C_Click(object sender, EventArgs e)
        {
            resetCalc();
            Array.Clear(calc.args, 0, 1);
            calc.disp = "0";
        }

        protected void btn_Backspase_Click(object sender, EventArgs e)
        {
            calc.disp = calc.deleteSymbol();
        }

        protected void Button8_Click(object sender, EventArgs e)
        {
            calc.arg = "";
            calc.disp = "0";
            //label1.Text = calc.disp;
            //label1.Font.Size = 50;
        }

        protected void btn_SQR_Click1(object sender, EventArgs e)
        {
            calc.symbol = "^";
            calc.fDeleg = new Calculator.CalcFunction().sqrOf;
            btn_Func_Click(calc.fDeleg, true);
        }

        protected void btn_SQRT_Click1(object sender, EventArgs e)
        {
            calc.symbol = "√";
            calc.fDeleg = new Calculator.CalcFunction().sqrtOf;
            btn_Func_Click(calc.fDeleg, true);
        }

        protected void btn_Negative_Click1(object sender, EventArgs e)
        {
            calc.minus = !calc.minus;
            inputVal('-');
        }


        protected void Button9_Click1(object sender, EventArgs e)
        {
            //this.CalcPannel.Visible = !this.CalcPannel.Visible;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                loadMe(calcList.IndexOf(calc.timeStampLabel));
                calcList.Clear();
                //HistoryList.Items.Clear();
            }
            catch { }

            //HistoryPannel.Visible = false;
            //CalcPannel.Enabled = true;
        }

        protected void btn_Ok2_Click(object sender, EventArgs e)
        {
            try
            {
                //getFromMR(listBox_MR.SelectedIndex + 1);
                getFromMR(mrList.IndexOf(calc.mrString) + 1);
                //MRPannel.Visible = false;
                //CalcPannel.Enabled = true;
            }
            catch
            { }

            //MRPannel.Visible = false;
            //CalcPannel.Enabled = true;
        }
    }
}