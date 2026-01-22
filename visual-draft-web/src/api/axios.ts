import axios from 'axios';

// 👇 ЗАМЕНИ ПОРТ НА СВОЙ ИЗ SWAGGER 👇
const BASE_URL = 'http://localhost:5048/api'; 

export const api = axios.create({
    baseURL: BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});