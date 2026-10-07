const API_URL = 'http://localhost:8080/api';

const token = localStorage.getItem('token');
const user = JSON.parse(localStorage.getItem('user'));

document.addEventListener('DOMContentLoaded', async () => {

    const userLogo = document.getElementById('log-out-icon-text');

    if (user) {      
        userLogo.innerText = user.name.charAt(0).toUpperCase() +
            user.surname.charAt(0).toUpperCase();
    }

    try {
        const response = await fetch(`${API_URL}/listados`, {
            method: 'GET',
            headers: {
                'Accept': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        })

        if (!response.ok) { throw new Error(`HTTP ${response.status}`); }

        const data = await response.json();

        const container = document.getElementById('lists-container');

        const list = document.createElement('ul');
        container.append(list);

        data.forEach((listado, i, Array) => {

            let listItem = document.createElement('li');
            list.append(listItem);

            let itemName = document.createElement('label');
            itemName.innerText = listado.name;
            listItem.append(itemName);

            let btnGenerateItem = document.createElement('button');
            btnGenerateItem.innerText = 'Generar';
            btnGenerateItem.classList.add('btn-generate');
            listItem.append(btnGenerateItem);

            let btnUseItem = document.createElement('button');
            btnUseItem.innerText = '¿Qué hace?';
            btnUseItem.classList.add('btn-use');
            listItem.append(btnUseItem);
        });

    } catch (error) {
        console.error('Error fetching listados:', error);
        window.location.href = '../errors/404-error.html';
    }
});