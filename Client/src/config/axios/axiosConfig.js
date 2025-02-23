import axios from "axios";

const instance = axios.create({
    baseURL: import.meta.env.VITE_API_URL
})

instance.interceptors.request.use((config) => {

    // const lang = localStorage.getItem('lang') || 'am';
    const token = localStorage.getItem('token') || null;

    config.headers = {
        'Authorization': `Bearer ${token}`,
        // 'Accept-Language': lang,
        ...(config.headers || {})
    };

    return config;
}

)


export default instance