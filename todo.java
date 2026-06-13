import java.io.*;
import java.nio.file.*;
import java.time.LocalDate;
import java.util.*;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.core.type.TypeReference;

class Task {
    public int id;
    public String title;
    public String description;
    public String priority;
    public String due;
    public boolean completed;

    public Task() {}
    public Task(int id, String title, String desc, String priority, String due, boolean completed) {
        this.id = id; this.title = title; this.description = desc; this.priority = priority; this.due = due; this.completed = completed;
    }
}

public class TodoApp {
    private static List<Task> tasks = new ArrayList<>();
    private static int nextId = 1;
    private static final String DATA_FILE = "tasks.json";
    private static ObjectMapper mapper = new ObjectMapper();

    public static void main(String[] args) throws IOException {
        load();
        Scanner sc = new Scanner(System.in);
        while (true) {
            System.out.println("\n=== Java Todo ===");
            System.out.println("1. Добавить\n2. Список всех\n3. Выполнить/вернуть\n4. Удалить\n5. Фильтр (активные/выполненные)\n0. Выход");
            System.out.print("> ");
            String opt = sc.nextLine();
            switch (opt) {
                case "1": addTask(sc); break;
                case "2": listAll(); break;
                case "3": toggleComplete(sc); break;
                case "4": deleteTask(sc); break;
                case "5": filterMenu(sc); break;
                case "0": System.out.println("До свидания!"); return;
                default: System.out.println("Неверно");
            }
        }
    }

    static void load() throws IOException {
        File f = new File(DATA_FILE);
        if (f.exists()) {
            String content = new String(Files.readAllBytes(Paths.get(DATA_FILE)));
            tasks = mapper.readValue(content, new TypeReference<List<Task>>(){});
            nextId = tasks.stream().mapToInt(t -> t.id).max().orElse(0) + 1;
        }
    }

    static void save() throws IOException {
        mapper.writeValue(new File(DATA_FILE), tasks);
    }

    static void addTask(Scanner sc) throws IOException {
        System.out.print("Название: ");
        String title = sc.nextLine().trim();
        if (title.isEmpty()) { System.out.println("Ошибка"); return; }
        System.out.print("Описание: ");
        String desc = sc.nextLine();
        System.out.print("Приоритет (high/medium/low) [medium]: ");
        String prio = sc.nextLine().toLowerCase();
        if (!prio.equals("high") && !prio.equals("low")) prio = "medium";
        System.out.print("Дедлайн (ГГГГ-ММ-ДД, Enter пропустить): ");
        String due = sc.nextLine().trim();
        tasks.add(new Task(nextId++, title, desc, prio, due, false));
        save();
        System.out.println("✓ Добавлена");
    }

    static void listAll() {
        if (tasks.isEmpty()) { System.out.println("Нет задач"); return; }
        for (Task t : tasks) {
            String status = t.completed ? "[✓]" : "[ ]";
            System.out.printf("%s %d: %s (%s) - %s [до %s]%n", status, t.id, t.title, t.priority, t.description, t.due);
        }
    }

    static void toggleComplete(Scanner sc) throws IOException {
        System.out.print("ID: ");
        int id = Integer.parseInt(sc.nextLine());
        for (Task t : tasks) {
            if (t.id == id) {
                t.completed = !t.completed;
                save();
                System.out.println("Статус изменён");
                return;
            }
        }
        System.out.println("Не найдено");
    }

    static void deleteTask(Scanner sc) throws IOException {
        System.out.print("ID для удаления: ");
        int id = Integer.parseInt(sc.nextLine());
        boolean removed = tasks.removeIf(t -> t.id == id);
        if (removed) { save(); System.out.println("Удалено"); }
        else System.out.println("Не найдено");
    }

    static void filterMenu(Scanner sc) {
        System.out.println("1. Активные\n2. Выполненные");
        String choice = sc.nextLine();
        for (Task t : tasks) {
            if (choice.equals("1") && !t.completed) System.out.printf("%d: %s%n", t.id, t.title);
            if (choice.equals("2") && t.completed) System.out.printf("%d: %s%n", t.id, t.title);
        }
    }
}
