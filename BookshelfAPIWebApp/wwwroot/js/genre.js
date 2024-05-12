const uriGenre = 'api/Genres';
let genres = [];

function getGenres() {
    fetch(uriGenre)
        .then(response => response.json())
        .then(data => _displayGenres(data))
        .catch(error => console.error('Неможливо отримати жанри.', error));
}

function addGenre() {
    const addNameTextbox = document.getElementById('add-name-genre');

    const genre = {
        name: addNameTextbox.value.trim(),
    };

    if (!genre.name) {
        showError('Введіть назву жанру');
        return;
    }

    fetch(uriGenre, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(genre)
    })
        .then(response => {
            if (!response.ok) {
                return response.text().then(errorText => {
                    throw new Error(errorText);
                });
            }
            return response.json();
        })
        .then(() => {
            getGenres();
            addNameTextbox.value = '';
        })
        .catch(error => {
            showError(error.message);
            console.error('Неможливо створити жанр.', error);
        });
}

function deleteGenre(id) {
    fetch(`${uriGenre}/${id}`, {
        method: 'DELETE'
    })
        .then(() => {
            getGenres();
            genres = genres.filter(genre => genre.id !== id);
            _displayGenres(genres);
        })
        .catch(error => console.error('Неможливо видалити жанр.', error));
}

function displayEditFormGenre(id) {
    const genre = genres.find(genre => genre.id === id);

    if (genre) {
        document.getElementById('edit-id-genre').value = genre.id || '';
        document.getElementById('edit-name-genre').value = genre.name || '';
    } else {
        document.getElementById('edit-id-genre').value = '';
        document.getElementById('edit-name-genre').value = '';
    }
    document.getElementById('editGenre').style.display = 'block';
}

function updateGenre() {
    const genreId = document.getElementById('edit-id-genre').value;
    const genreName = document.getElementById('edit-name-genre').value.trim();

    const genre = {
        id: parseInt(genreId, 10),
        name: genreName,
    };

    if (!genre.name) {
        showError('Введіть назву жанру');
        return;
    }

    fetch(`${uriGenre}/${genreId}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(genre)
    })
        .then(response => {
            if (!response.ok) {
                return response.text().then(errorText => {
                    throw new Error(errorText);
                });
            }
            getGenres();
            closeInputGenre();
        })
        .catch(error => {
            showError(error.message);
            console.error('Неможливо оновити жанр.', error);
        });

    return false;
}

function closeInputGenre() {
    document.getElementById('editGenre').style.display = 'none';
}

function _displayGenres(data) {
    const tBody = document.getElementById('genres');
    tBody.innerHTML = '';

    const button = document.createElement('button');

    data.forEach(genre => {
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Редагувати';
        editButton.classList.add('edit-btn', 'btn');
        editButton.setAttribute('onclick', `displayEditFormGenre(${genre.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Видалити';
        deleteButton.classList.add('delete-btn', 'btn');
        deleteButton.setAttribute('onclick', `deleteGenre(${genre.id})`);

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        let textNode = document.createTextNode(genre.name);
        td1.appendChild(textNode);

        let td2 = tr.insertCell(1);
        let textNodeBooksCount = document.createTextNode(genre.books.length);
        td2.appendChild(textNodeBooksCount);

        let td3 = tr.insertCell(2);
        td3.appendChild(editButton);

        let td4 = tr.insertCell(3);
        td4.appendChild(deleteButton);
    });

    genres = data;
}
