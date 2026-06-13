using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Spectre.Console;

namespace TodoApp
{
    class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; } // high, medium, low
        public string Due { get; set; }
        public bool Completed { get; set; }
    }

    class Program
    {
        static List<Task> tasks = new List<Task>();
        static int nextId = 1;
        const string DataFile = "tasks.json";

        static void Main()
        {
            Load();
            while (true)
            {
                AnsiConsole.Clear();
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold yellow]=== Todo Manager ===[/]")
                        .PageSize(8)
                        .AddChoices(new[] {
                            "➕ Добавить задачу",
                            "📋 Список всех",
                            "✅ Выполнить / восстановить",
                            "🗑 Удалить",
                            "🔍 Фильтр (активные/выполненные)",
                            "💾 Выход"
                        }));
                switch (choice)
                {
                    case "➕ Добавить задачу": AddTask(); break;
                    case "📋 Список всех": ListAll(); break;
                    case "✅ Выполнить / восстановить": ToggleComplete(); break;
                    case "🗑 Удалить": DeleteTask(); break;
                    case "🔍 Фильтр (активные/выполненные)": Filter(); break;
                    case "💾 Выход": return;
                }
                AnsiConsole.MarkupLine("\n[grey]Нажмите любую клавишу...[/]");
                Console.ReadKey();
            }
        }

        static void Load()
        {
            if (File.Exists(DataFile))
            {
                string json = File.ReadAllText(DataFile);
                tasks = JsonSerializer.Deserialize<List<Task>>(json);
                if (tasks.Count > 0) nextId = tasks.Max(t => t.Id) + 1;
            }
            else tasks = new List<Task>();
        }

        static void Save() => File.WriteAllText(DataFile, JsonSerializer.Serialize(tasks));

        static void AddTask()
        {
            var title = AnsiConsole.Ask<string>("[green]Название:[/]");
            if (string.IsNullOrWhiteSpace(title)) { AnsiConsole.MarkupLine("[red]Ошибка[/]"); return; }
            var desc = AnsiConsole.Ask<string>("[grey]Описание:[/]");
            var priority = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("Приоритет:").AddChoices("high", "medium", "low"));
            var due = AnsiConsole.Ask<string>("[grey]Дедлайн (ГГГГ-ММ-ДД, Enter пропустить):[/]");
            tasks.Add(new Task { Id = nextId++, Title = title, Description = desc, Priority = priority, Due = due, Completed = false });
            Save();
            AnsiConsole.MarkupLine("[green]✓ Добавлено[/]");
        }

        static void ListAll()
        {
            if (!tasks.Any()) { AnsiConsole.MarkupLine("[yellow]Нет задач[/]"); return; }
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Статус");
            table.AddColumn("Название");
            table.AddColumn("Приоритет");
            table.AddColumn("Дедлайн");
            foreach (var t in tasks)
            {
                string status = t.Completed ? "[green]✓[/]" : "[grey] [/]";
                string prioColor = t.Priority == "high" ? "red" : (t.Priority == "medium" ? "yellow" : "green");
                table.AddRow(t.Id.ToString(), status, t.Title, $"[{prioColor}]{t.Priority}[/]", t.Due ?? "-");
            }
            AnsiConsole.Render(table);
        }

        static void ToggleComplete()
        {
            int id = AnsiConsole.Ask<int>("ID задачи: ");
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task != null) { task.Completed = !task.Completed; Save(); AnsiConsole.MarkupLine("[green]Статус изменён[/]"); }
            else AnsiConsole.MarkupLine("[red]Не найдено[/]");
        }

        static void DeleteTask()
        {
            int id = AnsiConsole.Ask<int>("ID для удаления: ");
            if (tasks.RemoveAll(t => t.Id == id) > 0) { Save(); AnsiConsole.MarkupLine("[green]Удалено[/]"); }
            else AnsiConsole.MarkupLine("[red]Не найдено[/]");
        }

        static void Filter()
        {
            var filter = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("Фильтр:").AddChoices("Активные", "Выполненные"));
            var filtered = filter == "Активные" ? tasks.Where(t => !t.Completed) : tasks.Where(t => t.Completed);
            foreach (var t in filtered) AnsiConsole.MarkupLine($"{t.Id}: {t.Title}");
        }
    }
}
