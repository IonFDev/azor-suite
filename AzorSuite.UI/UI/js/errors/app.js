const token = localStorage.getItem('token');
const user = JSON.parse(localStorage.getItem('user'))

const userLogo = document.getElementById('log-out-logo');
userLogo.textContent = user.name.charAt(0).toUpperCase() + user.surname.charAt(0).toUpperCase();