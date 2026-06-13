package main

import (
	"bufio"
	"encoding/json"
	"fmt"
	"os"
	"strconv"
	"strings"
	"time"
)

type Task struct {
	ID          int    `json:"id"`
	Title       string `json:"title"`
	Description string `json:"description"`
	Priority    string `json:"priority"` // high, medium, low
	Due         string `json:"due"`
	Completed   bool   `json:"completed"`
}

var tasks []Task
var nextID = 1
const dataFile = "tasks.json"

func load() {
	file, err := os.ReadFile(dataFile)
	if err != nil { return }
	json.Unmarshal(file, &tasks)
	for _, t := range tasks { if t.ID >= nextID { nextID = t.ID + 1 } }
}

func save() {
	data, _ := json.MarshalIndent(tasks, "", "  ")
	os.WriteFile(dataFile, data, 0644)
}

func addTask() {
	reader := bufio.NewReader(os.Stdin)
	fmt.Print("Название: ")
	title, _ := reader.ReadString('\n')
	title = strings.TrimSpace(title)
	if title == "" { fmt.Println("Ошибка: название нужно"); return }
	fmt.Print("Описание: ")
	desc, _ := reader.ReadString('\n')
	fmt.Print("Приоритет (high/medium/low) [medium]: ")
	prio, _ := reader.ReadString('\n')
	prio = strings.TrimSpace(strings.ToLower(prio))
	if prio != "high" && prio != "low" { prio = "medium" }
	fmt.Print("Дедлайн (ГГГГ-ММ-ДД, необязательно): ")
	due, _ := reader.ReadString('\n')
	due = strings.TrimSpace(due)
	tasks = append(tasks, Task{ID: nextID, Title: title, Description: desc, Priority: prio, Due: due, Completed: false})
	nextID++
	save()
	fmt.Println("✅ Задача добавлена")
}

func listTasks() {
	if len(tasks) == 0 { fmt.Println("Нет задач."); return }
	fmt.Println("\n=== Список задач ===")
	for _, t := range tasks {
		status := " "
		if t.Completed { status = "✓" }
		prioSym := map[string]string{"high":"🔴","medium":"🟡","low":"🟢"}[t.Priority]
		fmt.Printf("[%s] %d: %s %s | %s | %s\n", status, t.ID, t.Title, prioSym, t.Description, t.Due)
	}
}

func toggleComplete() {
	fmt.Print("ID задачи: ")
	var id int
	fmt.Scanln(&id)
	for i, t := range tasks {
		if t.ID == id {
			tasks[i].Completed = !tasks[i].Completed
			save()
			fmt.Println("✓ Статус изменён")
			return
		}
	}
	fmt.Println("Задача не найдена")
}

func deleteTask() {
	fmt.Print("ID задачи для удаления: ")
	var id int
	fmt.Scanln(&id)
	for i, t := range tasks {
		if t.ID == id {
			tasks = append(tasks[:i], tasks[i+1:]...)
			save()
			fmt.Println("✓ Удалено")
			return
		}
	}
	fmt.Println("Не найдено")
}

func filterTasks() {
	fmt.Println("1. Все\n2. Активные\n3. Выполненные\n4. По приоритету")
	fmt.Print("Выбор: ")
	var choice int
	fmt.Scanln(&choice)
	switch choice {
	case 2:
		for _, t := range tasks { if !t.Completed { printTask(t) } }
	case 3:
		for _, t := range tasks { if t.Completed { printTask(t) } }
	case 4:
		fmt.Print("Приоритет (high/medium/low): ")
		var prio string
		fmt.Scanln(&prio)
		for _, t := range tasks { if t.Priority == prio { printTask(t) } }
	default:
		listTasks()
	}
}

func printTask(t Task) {
	fmt.Printf("%d: %s [%s] %s\n", t.ID, t.Title, t.Priority, t.Description)
}

func main() {
	load()
	reader := bufio.NewReader(os.Stdin)
	for {
		fmt.Println("\n=== Todo Manager ===")
		fmt.Println("1. Добавить\n2. Список всех\n3. Выполнить/вернуть\n4. Удалить\n5. Фильтр\n0. Выход")
		fmt.Print("> ")
		opt, _ := reader.ReadString('\n')
		opt = strings.TrimSpace(opt)
		switch opt {
		case "1": addTask()
		case "2": listTasks()
		case "3": toggleComplete()
		case "4": deleteTask()
		case "5": filterTasks()
		case "0": fmt.Println("До свидания!"); return
		default: fmt.Println("Неверный ввод")
		}
	}
}
