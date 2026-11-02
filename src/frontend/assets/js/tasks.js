const filters = document.querySelector('#tasks-filters');
const rows = document.querySelector('#tasks-rows');
const state = document.querySelector('#tasks-state');
const message = document.querySelector('#tasks-message');
const retry = document.querySelector('#retry-tasks');
const retryActions = document.querySelector('#retry-actions');
const previous = document.querySelector('#previous-page');
const next = document.querySelector('#next-page');
const pageNumber = document.querySelector('#page-number');

let page = 1;
let loading = false;
let canNext = false;
let profileLoaded = false;
let solvedTasks = '';

filters.addEventListener('submit', applyFilters);
filters.elements.difficulty.addEventListener('change', applyFilters);
filters.elements.sort.addEventListener('change', applyFilters);
filters.elements.hideSolved.addEventListener('change', applyFilters);

document.querySelector('#reset-filters').addEventListener('click', function () {
    filters.reset();
    page = 1;
    loadTasks();
});

previous.addEventListener('click', function () {
    if (loading || page <= 1) {
        return;
    }

    page -= 1;
    loadTasks();
});

next.addEventListener('click', function () {
    if (loading || !canNext) {
        return;
    }

    page += 1;
    loadTasks();
});

retry.addEventListener('click', function () {
    loadTasks();
});

startTasks();

async function startTasks() {
    setBusy(true);
    await loadSolvedTasks();
    await loadTasks();
}

function applyFilters(event) {
    event.preventDefault();

    if (loading) {
        return;
    }

    page = 1;
    loadTasks();
}

async function loadSolvedTasks() {
    const token = localStorage.getItem('token');

    if (!token) {
        filters.elements.hideSolved.title = 'Войдите, чтобы видеть решенные задания';
        return;
    }

    try {
        const response = await fetch('/api/user/profile/me', {
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });

        if (response.status === 401) {
            localStorage.removeItem('token');
            localStorage.removeItem('profileName');
            filters.elements.hideSolved.title = 'Войдите, чтобы видеть решенные задания';
            return;
        }

        if (!response.ok) {
            filters.elements.hideSolved.title = 'Что-то пошло не так';
            return;
        }

        const result = await response.json();

        if (!result.profile) {
            filters.elements.hideSolved.title = 'Что-то пошло не так';
            return;
        }

        solvedTasks = result.profile.tasksSolved || '';
        profileLoaded = true;
        filters.elements.hideSolved.title = 'Скрыть решенные задания';
    } catch (error) {
        filters.elements.hideSolved.title = 'Что-то пошло не так';
    }
}

async function loadTasks() {
    setBusy(true);

    rows.textContent = '';
    state.hidden = false;
    retryActions.hidden = true;
    message.textContent = 'Загрузка заданий...';
    pageNumber.textContent = 'Страница ' + page;
    canNext = false;

    let parameters = new URLSearchParams();
    parameters.set('page', page);

    let search = filters.elements.search.value.trim();
    let topic = filters.elements.topic.value.trim();
    let difficulty = filters.elements.difficulty.value;
    let sort = filters.elements.sort.value;

    if (search !== '') {
        parameters.set('search', search);
    }

    if (topic !== '') {
        parameters.set('topic', topic);
    }

    if (difficulty !== '') {
        parameters.set('difficulty', difficulty);
    }

    if (sort === 'easy' || sort === 'hard') {
        parameters.set('sortBy', '1');
        parameters.set('sortDescending', sort === 'hard');
    } else if (sort === 'order') {
        parameters.set('sortBy', '0');
        parameters.set('sortDescending', 'false');
    } else {
        parameters.set('sortBy', '2');
        parameters.set('sortDescending', sort !== 'oldest');
    }

    if (profileLoaded && filters.elements.hideSolved.checked) {
        parameters.set('hideSolved', 'true');
        parameters.set('solvedTasks', solvedTasks);
    }

    try {
        const response = await fetch('/api/tasks/?' + parameters.toString());

        if (response.status === 404) {
            showEmpty();
            return;
        }

        if (response.status === 400) {
            message.textContent = 'Некорректные фильтры';
            return;
        }

        if (!response.ok) {
            message.textContent = 'Не удалось загрузить задания';
            retryActions.hidden = false;
            return;
        }

        const result = await response.json();

        if (!Array.isArray(result.tasks)) {
            message.textContent = 'Ошибка на стороне сервера';
            retryActions.hidden = false;
            return;
        }

        if (result.tasks.length === 0) {
            showEmpty();
            return;
        }

        for (let task of result.tasks) {
            addTask(task);
        }

        canNext = result.tasks.length === 16 && page < 4000;
        state.hidden = true;
    } catch (error) {
        message.textContent = 'Не удалось загрузить задания';
        retryActions.hidden = false;
    } finally {
        setBusy(false);
    }
}

function addTask(task) {
    let link = document.createElement('a');
    link.className = 'task-row';
    link.href = '/task/' + task.id;

    if (profileLoaded && isSolved(task.id)) {
        link.className = 'task-row task-row-solved';
    }

    let name = document.createElement('span');
    name.className = 'task-name';
    name.textContent = task.id + '. ' + task.name;

    let difficulty = document.createElement('span');
    difficulty.textContent = task.difficulty;

    difficulty.className = 'task-difficulty difficulty-' + task.difficulty.trim().toLowerCase();

    link.append(name, difficulty);
    rows.append(link);
}

function isSolved(id) {
    if (solvedTasks === '') {
        return false;
    }

    let parts = solvedTasks.split(',');

    for (let part of parts) {
        if (part === '') {
            continue;
        }
        
        let range = part.split('-');
        let first = Number(range[0]);
        let last = first;

        if (range.length === 2) {
            last = Number(range[1]);
        }

        if (id >= first && id <= last) {
            return true;
        }
    }

    return false;
}

function showEmpty() {

    if (page > 1) {
        message.textContent = 'Пустая страница. Так получилось...';
    } else {
        message.textContent = 'Задания не найдены';
    }
}

function setBusy(value) {
    loading = value;

    for (let field of filters.elements) {
        field.disabled = value;
    }

    filters.elements.hideSolved.disabled = value || !profileLoaded;
    previous.disabled = value || page <= 1;
    next.disabled = value || !canNext;
    retry.disabled = value;
}
