const API_URL = 'http://localhost:8080/api';

const loginForm = document.getElementById('loginForm');
const result = document.getElementById('result');

// Check if the user is already logged in
if (localStorage.getItem('token')) {
    window.location.href = '/html/dashboard/index.html';
}

loginForm.addEventListener('submit', async (event) => {
    event.preventDefault();

    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    
    try {
        // Build the request to the API for login
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
        // Push the token an user data to localStorage and redirect to dashboard
        localStorage.setItem('token', data.token);
        localStorage.setItem('user', JSON.stringify(data.user));

        window.location.href = '/html/dashboard/index.html';

    } catch (error) {
        result.textContent =
            `Error conectando con la API: ${error.message}`;
    }
});