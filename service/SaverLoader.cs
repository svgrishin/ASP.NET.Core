using Newtonsoft.Json;

namespace core.start.service
{
    public abstract class SaverLoader
    {

        /// <summary>
        /// Сохранение состояния калькулятора в файл
        /// </summary>
        public static void saveCalc(Calculator c)
        {
            Calculator newC = new Calculator(c);

            newC.fDeleg = null;

            string s = JsonConvert.SerializeObject(newC);

            string location = Directory.GetCurrentDirectory() + "/calc.json";
            File.AppendAllText(location, s + "\n");
        }

        public static void btn_History_Click(string location, string resultstring)
        {
            string[] s = File.ReadAllLines(location);
            int i = 0;
            foreach (string str in s)
            {
                Array.Resize(ref HomeController.calcs, i + 1);
                Array.Resize(ref HomeController.resultStrings, i + 1);

                HomeController.calcs[i] = JsonConvert.DeserializeObject<Calculator>(str);

                HomeController.resultStrings[i] = HomeController.calcs[i].resultString;

                i++;
            }

            try
            {
                foreach (string c in HomeController.resultStrings)
                {
                    HomeController.calcList.Add(c);
                }
            }
            catch (Exception ex) { }
        }

    }
}
