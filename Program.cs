NoteManager manager = new NoteManager();
manager.LoadFromFile();

while (true)
{
    Console.WriteLine("Выберите одно из действий:" +
        "\n* Создать файл для заметок [0]" +
        "\n* Добавить заметку [1] " +
        "\n* Посмотреть все заметки [2]" +
        "\n* Удалить заметку [3]" +
        "\n* Поиск по ключевому слову [4]" +
        "\n* Редактировать заметку [5]" +
        "\n* Выйти [6]");
    int Choose = Convert.ToInt32(Console.ReadLine());
    if (Choose == 6)
        break;
    if (Choose >= 7)
        Console.WriteLine("Ты обезьяна? Сказано выбрато только из четырех!\n");
        

    switch (Choose)
    {
        case 0:
            manager.CreateFileNote();
            break;
        case 1: // Добавить заметку
            manager.AddNotes();
            break;
        case 2: // Посмотреть все заметки
            manager.ShowAllNotes();
            break;
        case 3: // Удалить заявку

            manager.DeleteNotes();
            break;
        case 4: //Поиск
            manager.SearchNotes();
            break;
        case 5: // Редактирование
            manager.EditNotes();
            break;
    }
}



class NoteManager
{
    private Dictionary<int, Note> strings = new Dictionary<int, Note>();
    private string filePath;
    public void LoadFromFile()
    {
        string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string fileName = "txt.txt";
        filePath = Path.Combine(MyDocuments, fileName);

        if (File.Exists(filePath))
        {
            strings = File.ReadLines(filePath).Select(line =>
            {
                var parts = line.Split('|');
                int id = int.Parse(parts[0]);
                string text = parts[1];
                DateTime date = DateTime.Parse(parts[2]);
                return new Note(id, text, date);
            }).ToDictionary(note => note.ID);
        }
    }
    public void CreateFileNote()
    {
        using (File.Create(filePath)) { }
    }
    public void AddNotes() // Добавить заметку
    {
        Console.WriteLine("Оставьте заметку");
        string? anywords = Console.ReadLine();
        int newId = strings.Any() ? strings.Keys.Max() + 1 : 1;
        Note newNote = new Note(newId, anywords, DateTime.Now);
        strings.Add(newId, newNote);
        try
        {
            File.WriteAllLines(filePath, strings.Values.Select(n => $"{n.ID}|{n.Text}|{n.DateAt}"));
            Console.WriteLine("*\nЗаметка успешно создана\n*\n");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Ошибка при записи: {ex.Message}");
        }
    }

    public void ShowAllNotes() // Посмотреть все заметки
    {
        if (strings == null || strings.Count == 0)
        {
            Console.WriteLine("Тут пока пусто...\n");
        }
        foreach (var i in strings.Values)
        {
            Console.WriteLine("----------");
            Console.WriteLine($"{i.ID}. {i.Text} ({i.DateAt})");
            Console.WriteLine("----------");
        }
    }
    public void DeleteNotes() // Удалить заявку
    {
        if (strings.Count == 0)
        {
            Console.WriteLine("Нет заметок для поиска");
            return;
        }
        Console.WriteLine("Выберите номер заметки для удаления");
        int ChooseDel = int.Parse(Console.ReadLine());
        Note noteToRemove = strings.Values.FirstOrDefault(n => n.ID == ChooseDel);

        if (strings.Remove(ChooseDel))
            Console.WriteLine("Заметка удалена");
        else
            Console.WriteLine("Заметка с таким ID не найдена");

        File.WriteAllLines(filePath, strings.Values.Select(n => $"{n.ID}|{n.Text}|{n.DateAt}"));
    }
    public void SearchNotes()//Поиск
    {
        if (strings.Count == 0)
        {
            Console.WriteLine("Нет заметок для поиска");
            return;
        }
        Console.WriteLine("Введите ключевое слово для поиска: ");
        string? keyword = Console.ReadLine();
        var SearchWord = strings.Values.Where(n => n.Text.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        if (SearchWord.Count == 0)
        {
            Console.WriteLine("Совпадений не найдено");
            return;
        }
        else
        {
            Console.WriteLine("Все найденные совпадения");
            foreach (var word in SearchWord)
            {
                Console.WriteLine("----------");
                Console.WriteLine($"{word.ID}. {word.Text} ({word.DateAt})");
                Console.WriteLine("----------");
            }
        }
    }
    public void EditNotes() // Редактирование
    {
        if (strings.Count == 0)
        {
            Console.WriteLine("Нет заметок для изменения");
            return;
        }
        try
        {
            Console.WriteLine("Для изменения записи введите её ID");
            int idKeyWord = int.Parse(Console.ReadLine());
            Note Searchid = strings.Values.FirstOrDefault(n => n.ID == idKeyWord);
            if (Searchid != null)
            {
                Console.WriteLine("++++++++++");
                Console.WriteLine($"{Searchid.ID}. {Searchid.Text} ({Searchid.DateAt})");
                Console.WriteLine("++++++++++\n");
                Console.WriteLine("Запись найдена, теперь можете внести изменение");
                string? editText = Console.ReadLine();
                Searchid.Text = editText;
                Searchid.DateAt = DateTime.Now;

                File.WriteAllLines(filePath, strings.Values.Select(n => $"{n.ID}|{n.Text}|{n.DateAt}"));

                Console.WriteLine("Запись была успешно измененна!");
            }
            else
                Console.WriteLine("Запись с таким ID не найдена");
        }
        catch
        {
            Console.WriteLine("Нужно писать ID арабскими цифрами");
        }
        ;
    }

}
public class Note
{
    public int ID { get; set; }
    public string Text { get; set; }
    public DateTime DateAt { get; set; }

    public Note(int id, string text, DateTime date)
    {
        ID = id;
        Text = text;
        DateAt = date;
    }
}
