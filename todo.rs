// dependencies in Cargo.toml:
// [dependencies]
// serde = { version = "1.0", features = ["derive"] }
// serde_json = "1.0"
// dialoguer = "0.11"
// chrono = "0.4"

use serde::{Serialize, Deserialize};
use std::fs;
use std::io;
use dialoguer::{Input, Select, Confirm};
use chrono::NaiveDate;

#[derive(Serialize, Deserialize, Clone)]
struct Task {
    id: u32,
    title: String,
    description: String,
    priority: String,
    due: Option<String>,
    completed: bool,
}

const DATA_FILE: &str = "tasks.json";

fn load_tasks() -> Vec<Task> {
    if let Ok(data) = fs::read_to_string(DATA_FILE) {
        serde_json::from_str(&data).unwrap_or_else(|_| Vec::new())
    } else {
        Vec::new()
    }
}

fn save_tasks(tasks: &Vec<Task>) {
    let data = serde_json::to_string_pretty(tasks).unwrap();
    fs::write(DATA_FILE, data).unwrap();
}

fn add_task(tasks: &mut Vec<Task>) {
    let title: String = Input::new().with_prompt("Название").interact_text().unwrap();
    if title.is_empty() { println!("Ошибка"); return; }
    let description: String = Input::new().with_prompt("Описание").default("".into()).interact_text().unwrap();
    let priority = ["Высокий", "Средний", "Низкий"];
    let prio_idx = Select::new().items(&priority).default(1).interact().unwrap();
    let priority_str = match prio_idx { 0 => "high", 1 => "medium", _ => "low" }.to_string();
    let due_str: String = Input::new().with_prompt("Дедлайн (ГГГГ-ММ-ДД, Enter пропустить)").allow_empty(true).interact_text().unwrap();
    let due = if due_str.is_empty() { None } else { Some(due_str) };
    let id = tasks.iter().map(|t| t.id).max().unwrap_or(0) + 1;
    tasks.push(Task { id, title, description, priority: priority_str, due, completed: false });
    save_tasks(tasks);
    println!("✅ Задача {} добавлена", id);
}

fn list_tasks(tasks: &Vec<Task>, filter: Option<&str>) {
    let filtered: Vec<&Task> = tasks.iter()
        .filter(|t| match filter {
            Some("active") => !t.completed,
            Some("completed") => t.completed,
            _ => true
        }).collect();
    if filtered.is_empty() { println!("Нет задач."); return; }
    for t in filtered {
        let status = if t.completed { "✓" } else { " " };
        let prio_sym = match t.priority.as_str() {
            "high" => "🔴", "medium" => "🟡", _ => "🟢"
        };
        let due = t.due.as_deref().unwrap_or("-");
        println!("[{}] {}: {} {} | {} | до {}", status, t.id, t.title, prio_sym, t.description, due);
    }
}

fn toggle_complete(tasks: &mut Vec<Task>) {
    let id: u32 = Input::new().with_prompt("ID задачи").interact_text().unwrap();
    if let Some(t) = tasks.iter_mut().find(|t| t.id == id) {
        t.completed = !t.completed;
        save_tasks(tasks);
        println!("Статус изменён");
    } else { println!("Не найдено"); }
}

fn delete_task(tasks: &mut Vec<Task>) {
    let id: u32 = Input::new().with_prompt("ID для удаления").interact_text().unwrap();
    let len = tasks.len();
    tasks.retain(|t| t.id != id);
    if tasks.len() < len { save_tasks(tasks); println!("Удалено"); }
    else { println!("Не найдено"); }
}

fn main() {
    let mut tasks = load_tasks();
    loop {
        println!("\n=== Rust Todo ===");
        let options = &["Добавить", "Список всех", "Активные", "Выполненные", "Выполнить/вернуть", "Удалить", "Выход"];
        let choice = Select::new().items(options).default(0).interact().unwrap();
        match choice {
            0 => add_task(&mut tasks),
            1 => list_tasks(&tasks, None),
            2 => list_tasks(&tasks, Some("active")),
            3 => list_tasks(&tasks, Some("completed")),
            4 => toggle_complete(&mut tasks),
            5 => delete_task(&mut tasks),
            _ => break,
        }
    }
}
