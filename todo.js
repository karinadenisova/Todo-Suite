<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Todo List · Продвинутый планировщик</title>
    <style>
        * { box-sizing: border-box; font-family: system-ui, 'Segoe UI', sans-serif; }
        body { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); margin: 0; min-height: 100vh; padding: 2rem; }
        .container { max-width: 1200px; margin: 0 auto; background: white; border-radius: 32px; box-shadow: 0 25px 50px -12px rgba(0,0,0,0.25); overflow: hidden; }
        .header { background: #2d3748; color: white; padding: 1.5rem 2rem; }
        .header h1 { margin: 0; font-weight: 600; display: flex; align-items: center; gap: 0.5rem; }
        .toolbar { background: #f7fafc; padding: 1rem 2rem; border-bottom: 1px solid #e2e8f0; display: flex; flex-wrap: wrap; gap: 1rem; align-items: center; }
        .search-box { flex: 2; min-width: 200px; }
        .search-box input { width: 100%; padding: 0.5rem; border: 1px solid #cbd5e0; border-radius: 8px; }
        .filter-group { display: flex; gap: 0.5rem; flex-wrap: wrap; }
        .filter-btn { background: #edf2f7; border: none; padding: 0.5rem 1rem; border-radius: 24px; cursor: pointer; transition: all 0.2s; }
        .filter-btn.active { background: #667eea; color: white; }
        .add-task { background: #f7fafc; padding: 1.5rem 2rem; border-bottom: 1px solid #e2e8f0; display: flex; gap: 1rem; flex-wrap: wrap; align-items: flex-end; }
        .add-task input, .add-task select, .add-task button { padding: 0.5rem; border-radius: 8px; border: 1px solid #cbd5e0; }
        .add-task input { flex: 3; min-width: 150px; }
        .add-task select { flex: 1; }
        .add-task button { background: #48bb78; color: white; border: none; cursor: pointer; font-weight: bold; }
        .task-list { padding: 1.5rem 2rem; background: white; }
        .task-card { background: #f9f9ff; border-radius: 16px; padding: 1rem; margin-bottom: 0.75rem; display: flex; align-items: center; gap: 1rem; transition: 0.1s; border-left: 6px solid; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
        .task-card.completed { opacity: 0.6; background: #e2e8f0; text-decoration: line-through; }
        .task-check { width: 24px; height: 24px; cursor: pointer; }
        .task-title { flex: 3; font-weight: 500; }
        .task-desc { flex: 4; color: #4a5568; font-size: 0.9rem; }
        .task-priority { width: 80px; font-size: 0.8rem; font-weight: bold; text-align: center; padding: 4px 8px; border-radius: 20px; }
        .task-due { width: 100px; font-size: 0.8rem; color: #2d3748; }
        .task-actions button { background: none; border: none; cursor: pointer; font-size: 1.2rem; margin: 0 4px; }
        .priority-high { border-left-color: #f56565; background: #fff5f5; }
        .priority-medium { border-left-color: #ed8936; background: #fffaf0; }
        .priority-low { border-left-color: #48bb78; background: #f0fff4; }
        .stats { background: #edf2f7; padding: 0.75rem 2rem; font-size: 0.9rem; color: #2d3748; display: flex; justify-content: space-between; }
        @media (max-width: 768px) {
            .task-card { flex-wrap: wrap; }
            .task-desc, .task-due { width: 100%; margin-top: 8px; }
        }
    </style>
</head>
<body>
<div class="container">
    <div class="header">
        <h1>📋 Планировщик задач · JavaScript</h1>
    </div>
    <div class="toolbar">
        <div class="search-box">
            <input type="text" id="searchInput" placeholder="🔍 Поиск по названию / описанию">
        </div>
        <div class="filter-group" id="filterGroup">
            <button class="filter-btn active" data-filter="all">Все</button>
            <button class="filter-btn" data-filter="active">Активные</button>
            <button class="filter-btn" data-filter="completed">Выполненные</button>
            <button class="filter-btn" data-filter="priority-high">🔥 Высокий</button>
            <button class="filter-btn" data-filter="priority-medium">🟠 Средний</button>
            <button class="filter-btn" data-filter="priority-low">🟢 Низкий</button>
        </div>
        <button id="exportBtn" style="margin-left:auto; background:#4299e1; border:none; border-radius:24px; padding:6px 12px; color:white;">📎 Экспорт CSV</button>
        <button id="importBtn" style="background:#9f7aea;">📂 Импорт CSV</button>
    </div>
    <div class="add-task">
        <input type="text" id="newTitle" placeholder="Название задачи *" required
