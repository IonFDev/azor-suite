const API_URL = 'http://localhost:8080/api';

const loginForm = document.getElementById('loginForm');
const result = document.getElementById('result');

loginForm.addEventListener('submit', async (event) => {
    event.preventDefault();

    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    try {
        const response = await fetch(`${API_URL}/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({
                username,
                password
            })
        });

        const data = await response.json();

        if (!response.ok) {
            result.textContent = data.message;
            return;
        }

        result.textContent =
            `Bienvenido, ${data.user.name}. Token recibido.`;

        console.log(data);
    } catch (error) {
        result.textContent =
            `Error conectando con la API: ${error.message}`;
    }
});