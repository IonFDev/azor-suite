const API_URL = 'http://localhost:8080/api';

const token = localStorage.getItem('token');
const user = JSON.parse(localStorage.getItem('user'));

const userLogo = document.getElementById('log-out-icon-text');
userLogo.innerText = user.name.charAt(0).toUpperCase() + user.surname.charAt(0).toUpperCase();