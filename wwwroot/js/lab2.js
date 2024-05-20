const uri = 'api/Participants';
const role_uri = 'api/Roles';

let participants = [];

function getParticipants() {
    fetch(uri)
        .then(response => response.json())
        .then(r => _displayParticipants(r.data))
        .catch(error => console.error('Неможливо отримати користувачів.', error));
}

function getRoles() {
    fetch(role_uri)
        .then(response => response.json())
        .then(r => _displayRoles(r.data))
        .catch(error => console.error('Неможливо отримати ролі.', error));
}

function addParticipant() {
    const addFirstNameTextbox = document.getElementById('add-firstName');
    const addLastNameTextbox = document.getElementById('add-lastName');
    const addEmailTextbox = document.getElementById('add-email');
    const addPhoneNumberTextbox = document.getElementById('add-phoneNumber');
    const addRoleSelect = document.getElementById('add-role');

    const participant = {
        firstName: addFirstNameTextbox.value.trim(),
        lastName: addLastNameTextbox.value.trim(),
        email: addEmailTextbox.value.trim(),
        phoneNumber: addPhoneNumberTextbox.value.trim(),
        roleId: addRoleSelect.value
    };

    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(participant)
    })
        .then(response => {
            if (!response.ok) {
                return response.json().then(error => { throw new Error(error.message); });
            }
            return response.json();
        })
        .then(() => {
            getParticipants();
            addFirstNameTextbox.value = '';
            addLastNameTextbox.value = '';
            addEmailTextbox.value = '';
            addPhoneNumberTextbox.value = '';

            document.getElementById('error-message').style.display = 'none';
        })
        .catch(error => {
            if (error.errors) {
                showError(`Помилки валідації: ${error.errors.join(', ')}`);
            } else {
                showError(`Помилка додавання користувача: ${error.message}`);
            }
            console.error('Неможливо додати користувача.', error);
        });
}

function deleteParticipant(id) {
    fetch(`${uri}/${id}`, {
        method: 'DELETE'
    })
        .then(() => getParticipants())
        .catch(error => console.error('Неможливо видалити користувача.', error));
}

function displayEditForm(id) {
    const participant = participants.find(p => p.participantId === id);

    document.getElementById('edit-id').value = participant.participantId;
    document.getElementById('edit-firstName').value = participant.firstName;
    document.getElementById('edit-lastName').value = participant.lastName;
    document.getElementById('edit-email').value = participant.email;
    document.getElementById('edit-phoneNumber').value = participant.phoneNumber;
    document.getElementById('edit-role').value = participant.roleId;
    document.getElementById('editForm').style.display = 'block';
}

function updateParticipant() {
    const participantId = document.getElementById('edit-id').value;

    const participant = {
        id: parseInt(participantId, 10),
        firstName: document.getElementById('edit-firstName').value.trim(),
        lastName: document.getElementById('edit-lastName').value.trim(),
        email: document.getElementById('edit-email').value.trim(),
        phoneNumber: document.getElementById('edit-phoneNumber').value.trim(),
        roleId: document.getElementById('edit-role').value.trim()
    };

    fetch(`${uri}/${participantId}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(participant)
    })
        .then(response => {
            if (!response.ok) {
                return response.json().then(error => { throw new Error(error.message); });
            }
            return response.json();
        })
        .then(() => {
            getParticipants();
           
            document.getElementById('error-message').style.display = 'none';
        })
        .catch(error => {
            showError(`Помилка оновлення інформації про користувача. ${error.message}`);
            console.error('Неможливо оновити інформацію про користувача.', error);
        });
    closeInput();

    return false;
}

function closeInput() {
    document.getElementById('editForm').style.display = 'none';
}


function _displayParticipants(data) {
    const tBody = document.getElementById('participants');
    tBody.innerHTML = '';


    const button = document.createElement('button');

    data.forEach(p => {
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Редагувати';
        editButton.setAttribute('onclick', `displayEditForm(${p.participantId})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Видалити';
        deleteButton.setAttribute('onclick', `deleteParticipant(${p.participantId})`);

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        let textNodeFirstName = document.createTextNode(p.firstName);
        td1.appendChild(textNodeFirstName);

        let td2 = tr.insertCell(1);
        let textNodeLastName = document.createTextNode(p.lastName);
        td2.appendChild(textNodeLastName);

        let td3 = tr.insertCell(2);
        let textNodeEmail = document.createTextNode(p.email);
        td3.appendChild(textNodeEmail);

        let td4 = tr.insertCell(3);
        let textNodePhoneNumber = document.createTextNode(p.phoneNumber);
        td4.appendChild(textNodePhoneNumber);

        let td5 = tr.insertCell(4);
        let roleName = '';
        if (p.roleId === 1) {
            roleName = 'Студент';
        } else if (p.roleId === 2) {
            roleName = 'Учитель';
        } else {
            roleName = 'Інша роль'; 
        }
        let textNodeRole = document.createTextNode(roleName);
        td5.appendChild(textNodeRole);

        let td6 = tr.insertCell(5);
        td6.appendChild(editButton);

        let td7 = tr.insertCell(6);
        td7.appendChild(deleteButton);
    });

    participants = data;
}

function _displayRoles(data) {
    const add_select = document.getElementById('add-role');
    const edit_select = document.getElementById('edit-role');
    data.forEach(r => {
        const opt_add = document.createElement('option');
        opt_add.value = r.roleId;
        opt_add.textContent = r.name;
        add_select.appendChild(opt_add);

        const opt_edit = document.createElement('option');
        opt_edit.value = r.roleId;
        opt_edit.textContent = r.name;
        edit_select.appendChild(opt_edit);
    });
}

function showError(message) {
    Swal.fire({
        icon: 'error',
        title: 'Помилка',
        text: message
    });
}

