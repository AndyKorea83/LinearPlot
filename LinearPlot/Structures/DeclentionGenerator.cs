namespace LinearPlot.Structures
{
    /// <summary>
    /// Хелпер, отвечающий за правильный падеж слова.
    /// </summary>
    internal static class DeclensionGenerator
    {
        /// <summary>
        /// Возвращает слова в падеже, зависимом от заданного числа
        /// </summary>
        /// <param name="number">Число от которого зависит выбранное слово</param>
        /// <param name="nominative">Именительный падеж слова. Например "день"</param>
        /// <param name="genetive">Родительный падеж слова. Например "дня"</param>
        /// <param name="plural">Множественное число слова. Например "дней"</param>
        /// <returns></returns>
        public static string Generate(int number, string nominative, string genetive, string plural)
        {
            string[] titles = new[] { nominative, genetive, plural };
            int[] cases = new[] { 2, 0, 1, 1, 1, 2 };
            int index = number % 100 > 4 && number % 100 < 20 ? 2 : cases[number % 10 < 5 ? number % 10 : 5];
            return titles[index];
        }
    }
}