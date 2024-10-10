using System;

namespace questionnaire
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random rng = new Random();
            int id = rng.Next(42, int.MaxValue);
            while (true)
            { 
                Console.WriteLine("Starting survey. Please, answer the following questions with utmost sincerity.");
                Console.WriteLine();
                var info = AskQuestions(id);
                Console.WriteLine();
                Console.WriteLine("Survey complete. Thank you for your time.");
                Console.WriteLine();
                Console.WriteLine("Please review provided information before proceeding.");
                Console.WriteLine();
                ShowAnswers(info);
                Console.WriteLine();
                if (AskYesOrNo("Do you confirm that above information is correct?"))
                {
                    Console.WriteLine("Thank you for your cooperation, citizen #{0}. Have a good day.", info.Item11);
                    Console.ReadKey();
                    break; 
                }
                else { Console.WriteLine("Oh no. What a pity. Looks like we're gonna have to do this all over.\n"); }
            }

        }

        static void ShowAnswers((string name, string surname, int age_bio, int age_chrono, DateTime birthdate,
            bool has_pets, byte pets_number, string[]? pets_names, byte fav_colours_number, string[]? fav_colours, int id) user)
        {
            Console.WriteLine($"Showing information on user #{user.id}:\n" +
                $"Full name: {user.name} {user.surname};\n" +
                $"Date of birth: {user.birthdate.ToString($"MMMM d, yyyy")};\n" +
                $"Chronological age: {user.age_chrono};\n" +
                $"Biological age: {user.age_bio};\n" +
                $"Favourite colours: {(user.fav_colours_number > 0? string.Join(", ", user.fav_colours) : "none, colour blind" )};\n" +
                $"Has pets: {(user.has_pets? user.pets_number : "no")}" +
                $"{(user.has_pets? (";\nName(s) of pet(s): " + string.Join(", ", user.pets_names)) : "")}.");
        }
        static (string, string, int, int, DateTime, bool, byte, string[]?, byte, string[]?, int) AskQuestions(int id)
        {
            (string name, string surname, int age_bio, int age_chrono, DateTime birthdate, bool has_pets, byte pets_number, string[]? pets_names, byte fav_colours_number, string[]? fav_colours, int id) info;
            info.id = id;
            info.name = AskName("What's your first name?");
            info.surname = AskName("What's your second name?");

            info.age_bio = AskNumber("How old are you?");
            info.birthdate = AskDate("When were you born?");
            DateTime current_date = DateTime.Now;
            info.age_chrono = current_date.Year - info.birthdate.Year;
            if (info.birthdate > current_date.AddYears(-info.age_chrono)) { --info.age_chrono; }

            info.has_pets = AskYesOrNo("Do you have any pets?");
            info.pets_names = null;
            if (info.has_pets)
            {
                info.pets_number = (byte)AskNumber("How many pets do you have?", 0, byte.MaxValue);
                if (info.pets_number == 0) { Console.WriteLine("So you don't have pets? Why did you say you did then?"); }
                else { info.pets_names = AskList("What's the name of your #{0} pet?", info.pets_number); }
            }
            else { info.pets_number = 0; }

            info.fav_colours_number = (byte)AskNumber("How many favourite colours do you have?", 0, byte.MaxValue);
            info.fav_colours = null;
            if (info.fav_colours_number == 0) { Console.WriteLine("Really? Are you colour blind?"); }
            else { info.fav_colours = AskList("What's your #{0} favourite colour?", info.fav_colours_number); }

            return info;
        }

        static string AskAgain(Func<string, string> AskQuestion, string question)
        {
            Console.WriteLine("This is not an adequate response. Let's try this again.");
            return AskQuestion(question);
        }
        static int AskAgain(Func<string, int, int, int> AskQuestion, string question, int min, int max)
        {
            Console.WriteLine("This is not an adequate response. Let's try this again.");
            return AskQuestion(question, min, max);
        }
        static bool AskAgain(Func<string, bool> AskQuestion, string question)
        {
            Console.WriteLine("This is not an adequate response. Let's try this again.");
            return AskQuestion(question);
        }
        static DateTime AskAgain(Func<string, DateTime> AskQuestion, string question)
        {
            Console.WriteLine("This is not an adequate response. Let's try this again.");
            return AskQuestion(question);
        }

        static string AskName(string question)
        { 
            Console.Write(question + " ");
            string name = Console.ReadLine();
            if (!char.IsLetter(name[0])) { return AskAgain(AskName, question); }
            try { return name[0].ToString().ToUpper() + name.Substring(1); }
            catch (IndexOutOfRangeException) { return AskAgain(AskName, question); }
        }
        static int AskNumber(string question, int min = 0, int max = int.MaxValue)
        {
            Console.Write(question + " ");
            if (int.TryParse(Console.ReadLine(), out int number))
            {
                if (number < min || number > max) { return AskAgain(AskNumber, question, min, max); }
                else { return number; }
            }
            else { return AskAgain(AskNumber, question, min, max); }
        }
        static bool AskYesOrNo(string question)
        {
            Console.Write(question + " [yes/no] ");
            string answer = Console.ReadLine().ToLower();
            if (answer == "yes" || answer == "y") { return true; }
            else if (answer == "no" || answer == "n") { return false; }
            else { return AskAgain(AskYesOrNo, question); }
        }
        static string[] AskList(string question, byte amount)
        {
            string[] list = new string[amount];
            for (int i = 0; i < amount; ++i) { list[i] = AskName(string.Format(question, i + 1)); }
            return list;
        }
        static DateTime AskDate(string question)
        {
            Console.Write(question + " [dd.mm.yyyy] ");
            string[] date_input = Console.ReadLine().Split(".");
            try
            {
                DateTime date = new DateTime(int.Parse(date_input[2]), int.Parse(date_input[1]), int.Parse(date_input[0]));
                return date;
            }
            catch (Exception ex) when (ex is ArgumentOutOfRangeException ||
                                       ex is IndexOutOfRangeException ||
                                       ex is FormatException)
            { return AskAgain(AskDate, question); }
        }
    }
}
