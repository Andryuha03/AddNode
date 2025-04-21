using System.Collections.Immutable;
using System.Diagnostics.Tracing;
using System.Security.Cryptography;
while (true)
{
    List<Note> strings = new List<Note>();
    
    string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    string fileName = "txt.txt";
    string filePath = Path.Combine(MyDocuments, fileName);

    if (File.Exists(filePath))
    {
        strings = File.ReadLines(filePath).Select(line =>
        {
            var parts = line.Split('|');                
            int id = int.Parse(parts[0]);
            string text = parts[1];
            DateTime date = DateTime.Parse(parts[2]);
            return new Note(id, text, date);
        }).ToList();
    }

    string? anywords;

    Console.WriteLine("Выберите одно из действий:\n* Создать файл для заметок [0]\n* Добавить заметку [1] \n* Посмотреть все заметки [2]\n* Удалить заметку [3]\n* Поиск по ключевому слову [4]\n* Выйти [5]");
    int Choose = Convert.ToInt32(Console.ReadLine());
    if (Choose == 5)
        break;
    if (Choose >= 6)
        Console.WriteLine("Ты обезьяна? Сказано выбрато только из четырех!\n");
        

    switch (Choose)
    {
        case 0:
            using (File.Create(filePath)) { }
            break;
        case 1:

            Console.WriteLine("Оставьте заметку");
            anywords = Console.ReadLine();
            int newId = strings.Any() ? strings.Max(n => n.ID) + 1 : 1;
            Note newNote = new Note(newId, anywords, DateTime.Now);
            strings.Add(newNote);

            File.WriteAllLines(filePath, strings.Select(n => $"{n.ID}|{n.Text}|{n.DateAt}"));
            
            break;
        case 2:
            if (strings == null || strings.Count == 0)
            {
                Console.WriteLine("Тут пока пусто...\n");
            }
            foreach (var i in strings)
            {
                Console.WriteLine("----------");
                Console.WriteLine($"{i.ID}. {i.Text} ({i.DateAt})");
                Console.WriteLine("----------");
            }
            break;
        case 3:
            if (strings.Count == 0)
            {
                Console.WriteLine("Нет заметок для удаления");
                break;
            }
            Console.WriteLine("Выберите номер заметки для удаления");
            int ChooseDel = int.Parse(Console.ReadLine());
            Note noteToRemove = strings.FirstOrDefault(n => n.ID == ChooseDel);

            if (noteToRemove != null)
            {
                strings.Remove(noteToRemove);
                Console.WriteLine("Заметка удалена");
            }
            else
            Console.WriteLine("Заметка с таким ID не найдена");
            File.WriteAllLines(filePath, strings.Select(n => $"{n.ID}|{n.Text}|{n.DateAt}"));
            break;
        case 4:
            Console.WriteLine("Введите ключевое слово для поиска: ");
            string? keyword = Console.ReadLine();
            var SearchWord = strings.Where(n => n.Text.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            Console.WriteLine("Все найденные совпадения");
            foreach (var word in SearchWord)
            {
                Console.WriteLine("----------");
                Console.WriteLine($"{word.ID}. {word.Text} ({word.DateAt})");
                Console.WriteLine("----------");
            }

            break;
    }
}
class Note
{
    public int ID {  get; set; }
    public string Text { get; set; }
    public DateTime DateAt { get; set; }

    public Note(int id, string text, DateTime date) {
        ID = id;
        Text = text;
        DateAt = date;
    }
}