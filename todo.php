<?php
// todo.php
$dataFile = 'tasks.json';
$tasks = file_exists($dataFile) ? json_decode(file_get_contents($dataFile), true) : [];

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $action = $_POST['action'] ?? '';
    if ($action === 'add') {
        $tasks[] = [
            'id' => count($tasks) ? max(array_column($tasks, 'id')) + 1 : 1,
            'title' => $_POST['title'],
            'description' => $_POST['description'],
            'priority' => $_POST['priority'],
            'due' => $_POST['due'],
            'completed' => false
        ];
        file_put_contents($dataFile, json_encode($tasks, JSON_PRETTY_PRINT));
        header('Location: todo.php');
        exit;
    } elseif ($action === 'toggle') {
        $id = $_POST['id'];
        foreach ($tasks as &$t) { if ($t['id'] == $id) { $t['completed'] = !$t['completed']; break; } }
        file_put_contents($dataFile, json_encode($tasks, JSON_PRETTY_PRINT));
        header('Location: todo.php');
        exit;
    } elseif ($action === 'delete') {
        $id = $_POST['id'];
        $tasks = array_filter($tasks, fn($t) => $t['id'] != $id);
        file_put_contents($dataFile, json_encode(array_values($tasks), JSON_PRETTY_PRINT));
        header('Location: todo.php');
        exit;
    }
}
?>
<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <title>PHP Todo List</title>
    <style>
        body { font-family: Arial; background: #f0f2f5; margin: 30px; }
        .container { max-width: 1000px; margin: auto; background: white; border-radius: 16px; padding: 20px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
        h1 { color: #2c3e50; }
        form { display: flex; gap: 8px; flex-wrap: wrap; margin-bottom: 20px; }
        input, select, button { padding: 8px; border-radius: 8px; border: 1px solid #ccc; }
        button { background: #3498db; color: white; border: none; cursor: pointer; }
        .task { background: #f9f9f9; margin: 8px 0; padding: 12px; border-radius: 8px; display: flex; align-items: center; gap: 12px; border-left: 6px solid; }
        .task.completed { opacity: 0.6; text-decoration: line-through; background: #e9ecef; }
        .priority-high { border-left-color: #e74c3c; }
        .priority-medium { border-left-color: #f39c12; }
        .priority-low { border-left-color: #2ecc71; }
        .actions button { background: none; border: none; font-size: 1.2rem; margin: 0 4px; cursor: pointer; }
    </style>
</head>
<body>
<div class="container">
    <h1>📋 PHP Планировщик</h1>
    <form method="POST">
        <input type="hidden" name="action" value="add">
        <input type="text" name="title" placeholder="Название" required>
        <input type="text" name="description" placeholder="Описание">
        <select name="priority">
            <option value="high">🔴 Высокий</option>
            <option value="medium">🟡 Средний</option>
            <option value="low">🟢 Низкий</option>
        </select>
        <input type="date" name="due">
        <button type="submit">➕ Добавить</button>
    </form>
    <?php foreach ($tasks as $t): ?>
        <div class="task <?= $t['completed'] ? 'completed' : '' ?> priority-<?= $t['priority'] ?>">
            <form method="POST" style="margin:0;">
                <input type="hidden" name="action" value="toggle">
                <input type="hidden" name="id" value="<?= $t['id'] ?>">
                <button type="submit" style="background:none; border:none; font-size:1.2rem;"><?= $t['completed'] ? '✅' : '⬜' ?></button>
            </form>
            <div style="flex:2;"><strong><?= htmlspecialchars($t['title']) ?></strong><br><small><?= htmlspecialchars($t['description']) ?></small></div>
            <div style="width:80px;"><?= $t['priority'] ?></div>
            <div style="width:100px;"><?= $t['due'] ?? '—' ?></div>
            <div class="actions">
                <form method="POST" style="display:inline;">
                    <input type="hidden" name="action" value="delete">
                    <input type="hidden" name="id" value="<?= $t['id'] ?>">
                    <button type="submit">🗑️</button>
                </form>
            </div>
        </div>
    <?php endforeach; ?>
    <?php if (empty($tasks)) echo '<p>Нет задач. Добавьте первую!</p>'; ?>
</div>
</body>
</html>
