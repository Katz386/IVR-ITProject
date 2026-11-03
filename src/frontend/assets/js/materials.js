const message = document.querySelector('#materials-message');
const content = document.querySelector('#materials-content');

loadMaterials();

async function loadMaterials() {
    try {
        let response = await fetch('/assets/md/materials.md');

        if (!response.ok) {
            message.textContent = 'Не удалось загрузить материалы';
            return;
        }

        let text = await response.text();
        renderMarkdown(text, content);
        content.hidden = false;
        message.hidden = true;
    } catch (error) {
        message.textContent = 'Не удалось загрузить материалы';
    }
}

function renderMarkdown(text, element) {
    let html = marked.parse(text);
    let cleanHtml = DOMPurify.sanitize(html);

    element.innerHTML = cleanHtml;
}