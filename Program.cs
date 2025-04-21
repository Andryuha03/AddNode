using System.Collections.Immutable;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.VisualBasic;
while (true)
{
    Dictionary<int,Note> strings = new Dictionary<int, Note>();
    
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
        }).ToDictionary(note => note.ID);
    }

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
            using (File.Create(filePath)) { }
            break;
        case 1: // Добавить заметку
            AddNotes(strings, filePath);
            break;
        case 2: // Посмотреть все заметки
            ShowAllNotes(strings);
            break;
        case 3: // Удалить заявку
            if (strings.Count == 0)
            {
                Console.WriteLine("Нет заметок для удаления");
                break;
            }
            DeleteNotes(strings, filePath);
            break;
        case 4: //Поиск
            if (strings.Count == 0)
            {
                Console.WriteLine("Нет заметок для поиска");
                break;
            }
            SearchNotes(strings);
            break;
        case 5: // Редактирование
            if (strings.Count == 0)
            {
                Console.WriteLine("Нет заметок для изменения");
                break;
            }
            EditNotes(strings, filePath);
            break;
    }
}

void AddNotes(Dictionary<int, Note> notes, string path) // Добавить заметку
{
    Console.WriteLine("Оставьте заметку");
    string? anywords = Console.ReadLine();
    int newId = notes.Any() ? notes.Keys.Max() + 1 : 1;
    Note newNote = new Note(newId, anywords, DateTime.Now);
    notes.Add(newId, newNote);
    try {
        File.WriteAllLines(path, notes.Values.Select(n => $"{n.ID}|{n.Text}|{n.DateAt}"));
        Console.WriteLine("*\nЗаметка успешно создана\n*\n");
    }
    catch(IOException ex) {
        Console.WriteLine($"Ошибка при записи: {ex.Message}");
    }
}

void ShowAllNotes(Dictionary<int, Note> notes) // Посмотреть все заметки
{
    if (notes == null || notes.Count == 0)
    {
        Console.WriteLine("Тут пока пусто...\n");
    }
    foreach (var i in notes.Values)
    {
        Console.WriteLine("----------");
        Console.WriteLine($"{i.ID}. {i.Text} ({i.DateAt})");
        Console.WriteLine("----------");
    }
}
void DeleteNotes(Dictionary<int, Note> notes, string path) // Удалить заявку
{
    Console.WriteLine("Выберите номер заметки для удаления");
    int ChooseDel = int.Parse(Console.ReadLine());
    Note noteToRemove = notes.Values.FirstOrDefault(n => n.ID == ChooseDel);

    if (noteToRemove != null)
    {
        notes.Remove(ChooseDel);
        Console.WriteLine("Заметка удалена");
    }
    else
        Console.WriteLine("Заметка с таким ID не найдена");
    
    File.WriteAllLines(path, notes.Values.Select(n => $"{n.ID}|{n.Text}|{n.DateAt}"));
}
void SearchNotes(Dictionary<int, Note> notes)//Поиск
{
    Console.WriteLine("Введите ключевое слово для поиска: ");
    string? keyword = Console.ReadLine();
    var SearchWord = notes.Values.Where(n=>n.Text.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
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
void EditNotes(Dictionary<int, Note> notes, string path) // Редактирование
{
    try
    {
        Console.WriteLine("Для изменения записи введите её ID");
        int idKeyWord = int.Parse(Console.ReadLine());
        Note Searchid = notes.Values.FirstOrDefault(n => n.ID == idKeyWord);
        if (Searchid != null)
        {
            Console.WriteLine("++++++++++");
            Console.WriteLine($"{Searchid.ID}. {Searchid.Text} ({Searchid.DateAt})");
            Console.WriteLine("++++++++++\n");
            Console.WriteLine("Запись найдена, теперь можете внести изменение");
            string? editText = Console.ReadLine();
            Searchid.Text = editText;
            Searchid.DateAt = DateTime.Now;

            File.WriteAllLines(path, notes.Values.Select(n => $"{n.ID}|{n.Text}|{n.DateAt}"));

            Console.WriteLine("Запись была успешно измененна!");
        }
        else
            Console.WriteLine("Запись с таким ID не найдена");
    }
    catch
    {
        Console.WriteLine("Нужно писать ID арабскими цифрами");
    };
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