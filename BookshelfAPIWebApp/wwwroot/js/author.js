const uriAuthor = 'api/Authors';
let authors = [];

function getAuthors() {
    fetch(uriAuthor)
        .then(response => response.json())
        .then(data => _displayAuthors(data))
        .catch(error => console.error('Неможливо отримати авторів.', error));
}

function addAuthor() {
    const addNameTextbox = document.getElementById('add-name-author');
    const addDescriptionTextbox = document.getElementById('add-description-author');
    const addBirthdayTextbox = document.getElementById('add-birthday-author');

    const author = {
        name: addNameTextbox.value.trim(),
        description: addDescriptionTextbox.value.trim() === '' ? '' : addDescriptionTextbox.value.trim(),
        birthday: addBirthdayTextbox.value.trim() === '' ? null : addBirthdayTextbox.value.trim(),
    };

    fetch(uriAuthor, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(author)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Неможливо створити автора.');
            }
            return response.json();
        })
        .then(() => {
            getAuthors();
            addNameTextbox.value = '';
            addDescriptionTextbox.value = '';
            addBirthdayTextbox.value = '';
        })
        .catch(error => {
            showError(error.message);
            console.error('Неможливо створити автора.', error);
        });
}

function deleteAuthor(id) {
    fetch(`${uriAuthor}/${id}`, {
        method: 'DELETE'
    })
        .then(() => {
            getAuthors();
            authors = authors.filter(author => author.id !== id);
            _displayAuthors(authors);
        })
        .catch(error => console.error('Неможливо видалити автора.', error));
}


function displayEditFormAuthor(id) {
    const author = authors.find(author => author.id === id);

    document.getElementById('edit-id-author').value = author.id;
    document.getElementById('edit-name-author').value = author.name;
    document.getElementById('edit-description-author').value = author.description;
    document.getElementById('edit-birthday-author').value = author.birthday;

    const rowIndex = authors.findIndex(author => author.id === id);
    const table = document.getElementById('authors').parentNode;
    const row = table.rows[rowIndex + 1];

    const nextRow = row.nextSibling;

    const editContainer = document.getElementById('editAuthor');

    if (nextRow) {
        nextRow.parentNode.insertBefore(editContainer, nextRow);
    } else {
        table.appendChild(editContainer);
    }

    editContainer.style.display = 'block';
}

function updateAuthor() {
    const authorId = document.getElementById('edit-id-author').value;
    const authorName = document.getElementById('edit-name-author').value.trim();
    const authorDescription = document.getElementById('edit-description-author').value.trim() === '' ? '' : document.getElementById('edit-description-author').value.trim();
    const authorBirthday = document.getElementById('edit-birthday-author').value.trim();

    const author = {
        id: parseInt(authorId, 10),
        name: authorName,
        description: authorDescription,
        birthday: authorBirthday
    };

    fetch(`${uriAuthor}/${authorId}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(author)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Неможливо оновити автора. Автор з таким іменем вже існує.');
            }
            getAuthors();
            closeInputAuthor();
        })
        .catch(error => {
            showError(error.message);
            console.error('Неможливо оновити автора.', error);
        });

    return false;
}

function closeInputAuthor() {
    document.getElementById('editAuthor').style.display = 'none';
}

function _displayAuthors(data) {
    const tBody = document.getElementById('authors');
    tBody.innerHTML = '';

    const button = document.createElement('button');

    data.forEach(author => {
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Редагувати';
        editButton.classList.add('edit-btn', 'btn');
        editButton.setAttribute('onclick', `displayEditFormAuthor(${author.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Видалити';
        deleteButton.classList.add('delete-btn', 'btn');
        deleteButton.setAttribute('onclick', `deleteAuthor(${author.id})`);

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        let textNode = document.createTextNode(author.name);
        td1.appendChild(textNode);

        let td2 = tr.insertCell(1);
        let textNodeDescription = document.createTextNode(author.description);
        td2.appendChild(textNodeDescription);

        let td3 = tr.insertCell(2);
        let textNodeBirthday = document.createTextNode(author.birthday);
        td3.appendChild(textNodeBirthday);

        let td4 = tr.insertCell(3);
        td4.appendChild(editButton);

        let td5 = tr.insertCell(4);
        td5.appendChild(deleteButton);
    });

    authors = data;
}
