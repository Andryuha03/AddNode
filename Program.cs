using System.Security.Cryptography;
while (true)
{
    List<string> strings = new List<string>();
    string? anywords;

    Console.WriteLine("Выберите одно из действий:\n* Создать файл для заметок [0]\n* Добавить заметку [1] \n* Посмотреть все заметки [2]\n* Удалить заметку [3] \n* Выйти [4]");
    int Choose = Convert.ToInt32(Console.ReadLine());
    if (Choose == 4)
        break;
    switch (Choose)
    {
        case 0:
            string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fileName = "txt.txt";
            string filePath = Path.Combine(MyDocuments, fileName);


            break;
        case 1:

            Console.WriteLine("Оставьте заметку");
            anywords = Console.ReadLine();
            strings.Add(anywords);
            break;
        case 2:
            foreach (string s in strings)
            {
                Console.Write(strings.Count);
                Console.WriteLine(s);
            }
            break;
        case 3:

            break;
        case 4:

            break;
    }
}