const uri = 'api/Courses';
const levels_uri = 'api/Levels';
const categories_uri = 'api/Categories';
const languages_uri = 'api/Languages';
const participants_uri = 'api/Participants';

let courses = [];

function getCourses() {
    fetch(uri)
        .then(response => response.json())
        .then(data => _displayCourses(data))
        .catch(error => console.error('Помилка в отриманні курсів.', error));
}

function getLevels() {
    fetch(levels_uri)
        .then(response => response.json())
        .then(data => _displayLevels(data))
        .catch(error => console.error('Помилка в отриманні рівнів курсу.', error));
}

function getCategories() {
    fetch(categories_uri)
        .then(response => response.json())
        .then(data => _displayCategories(data))
        .catch(error => console.error('Помилка в отриманні категорій.', error));
}

function getLanguages() {
    fetch(languages_uri)
        .then(response => response.json())
        .then(data => _displayLanguages(data))
        .catch(error => console.error('Помилка в отриманні мов.', error));
}

function getParticipants() {
    fetch(participants_uri)
        .then(response => response.json())
        .then(data => _displayParticipants(data))
        .catch(error => console.error('Помилка в отриманні учасників курсу.', error));
}

function addCourse() {
    const addNameTextbox = document.getElementById('add-name');
    const addInfoTextbox = document.getElementById('add-info');
    const addLevelTextbox = document.getElementById('add-level');
    const addCategoryTextbox = document.getElementById('add-category');
    const addLanguageTextbox = document.getElementById('add-language');
    const addTeacherTextbox = document.getElementById('add-teacher');

    const course = {
        name: addNameTextbox.value.trim(),
        info: addInfoTextbox.value.trim(),
        levelId: addLevelTextbox.value,
        categoryId: addCategoryTextbox.value,
        languageId: addLanguageTextbox.value,
        participantId: addTeacherTextbox.value,
    };

    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(course)
    })
        .then(response => response.json())
        .then(() => {
            getCourses();
            addNameTextbox.value = '';
            addInfoTextbox.value = '';
        })
        .catch(error => console.error('Помилка в додаванні курсу.', error));
}

function deleteCourse(id) {
    fetch(`${uri}/${id}`, {
        method: 'DELETE'
    })
        .then(() => getCourses())
        .catch(error => console.error('Помилка у видаленні курсу.', error));
}

function displayEditForm(id) {
    const course = courses.find(course => course.id === id);

    document.getElementById('edit-id').value = course.id;
    document.getElementById('edit-name').value = course.name;
    document.getElementById('edit-info').value = course.info;
    document.getElementById('edit-level').value = course.levelId;
    document.getElementById('edit-categories').value = course.categories;
    document.getElementById('edit-languages').value = course.languages;
    document.getElementById('edit-participants').value = course.participants;
    document.getElementById('editForm').style.display = 'block';
}

function updateCourse() {
    const courseId = document.getElementById('edit-id').value;
    const course = {
        id: parseInt(courseId, 10),
        name: document.getElementById('edit-name').value.trim(),
        info: document.getElementById('edit-info').value.trim(),
        levelId: document.getElementById('edit-level').value.trim(),
        categories: document.getElementById('edit-category').value.trim(),
        languages: document.getElementById('edit-language').value.trim(),
        participants: document.getElementById('edit-teacher').value.trim(),
    };

    fetch(`${uri}/${courseId}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(course)
    })
        .then(() => getCourses())
        .catch(error => console.error('Помилка в оновленні курсу.', error));

    closeInput();

    return false;
}

function closeInput() {
    document.getElementById('editForm').style.display = 'none';
}


function _displayCourses(data) {
    const tBody = document.getElementById('courses');
    tBody.innerHTML = '';


    const button = document.createElement('button');

    data.forEach(course => {
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditForm(${course.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteCourse(${course.id})`);

        let tr = tBody.insertRow();


        let td1 = tr.insertCell(0);
        let textNode = document.createTextNode(course.name);
        td1.appendChild(textNode);

        let td2 = tr.insertCell(1);
        let textNodeInfo = document.createTextNode(course.info);
        td2.appendChild(textNodeInfo);

        let td3 = tr.insertCell(2);
        let textNodeLevel = document.createTextNode(course.level);
        td3.appendChild(textNodeLevel);

        let td4 = tr.insertCell(3);
        let textNodeCategory = document.createTextNode(course.categories);
        td4.appendChild(textNodeCategory);

        let td5 = tr.insertCell(4);
        let textNodeLanguage = document.createTextNode(course.languages);
        td5.appendChild(textNodeLanguage);

        let td6 = tr.insertCell(5);
        let textNodeParticipant = document.createTextNode(course.participants);
        td6.appendChild(textNodeParticipant);

        let td7 = tr.insertCell(6);
        td7.appendChild(editButton);

        let td8 = tr.insertCell(7);
        td8.appendChild(deleteButton);
    });

    courses = data;
}

function _displayCategories(data) {
    const add_select = document.getElementById('add-category');
    const edit_select = document.getElementById('edit-category');
    data.forEach(c => {
        const opt_add = document.createElement('option');
        opt_add.value = c.categoryId;
        opt_add.textContent = c.name;
        add_select.appendChild(opt_add);

        const opt_edit = document.createElement('option');
        opt_edit.value = c.categoryId;
        opt_edit.textContent = c.name;
        edit_select.appendChild(opt_edit);
    });
}
