using System.Security.Cryptography;
while (true)
{
    List<string> strings = new List<string>();
    
    string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    string fileName = "txt.txt";
    string filePath = Path.Combine(MyDocuments, fileName);

    if (File.Exists(filePath))
    {
        strings = File.ReadAllLines(filePath).ToList();
    }

    string? anywords;

    Console.WriteLine("Выберите одно из действий:\n* Создать файл для заметок [0]\n* Добавить заметку [1] \n* Посмотреть все заметки [2]\n* Удалить заметку [3] \n* Выйти [5]");
    int Choose = Convert.ToInt32(Console.ReadLine());
    if (Choose == 4)
        break;
    if (Choose >= 5)
        Console.WriteLine("Ты обезьяна? Сказано выбрато только из четырех!\n");
        

    switch (Choose)
    {
        case 0:
            File.Create(filePath);
            break;
        case 1:

            Console.WriteLine("Оставьте заметку");
            anywords = Console.ReadLine();
            strings.Add(anywords);
            File.WriteAllLines(filePath, strings);
            
            break;
        case 2:
            if (strings == null || strings.Count == 0)
            {
                Console.WriteLine("Тут пока пусто...\n");
            }
            for (int i = 0;i < strings.Count;i++)
            {
                Console.WriteLine("----------");
                Console.WriteLine($"{i+1}. {strings[i]}");
                Console.WriteLine("----------");
            }
            break;
        case 3:
            File.WriteAllText(filePath, String.Empty);
            Console.WriteLine("Выберите номер заметки для удаления");
            int ChooseDel = Convert.ToInt32(Console.ReadLine());
            strings.RemoveAt(ChooseDel-1);
            File.WriteAllLines(filePath, strings);
            break;
    }
}