import axios from 'axios';

// axios.defaults.baseURL= "http://localhost:5058"

axios.defaults.baseURL = "https://todolistserver-it24.onrender.com";

// --- : הוספת טוקן לכל בקשה יוצאת ---
axios.interceptors.request.use(config => {
  const token = localStorage.getItem("token");
  if (token) {
      config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// --- : טיפול בשגיאת 401 (התחברות פגה) ---
axios.interceptors.response.use(
  response => response,
  error => {
      console.error('Error response:', error.response);
      if (error.response && error.response.status === 401) {
          localStorage.removeItem("token"); // מחיקת הטוקן הלא תקין
          window.location.href = "/login"; // הפניה לדף לוגין
      }
      return Promise.reject(error);
  }
);


export default {
  login: async (username, password) => {
    const result = await axios.post("/login", { username, password });
    localStorage.setItem("token", result.data.token);
    localStorage.setItem("userName", username); 
    return result.data;
  },

  register: async (username, password) => {
      await axios.post("/register", { username, password });
  },

  getTasks: async () => {
    const result = await axios.get(`/tasks`)    
    return result.data;
  },

  addTask: async(name)=>{
    console.log('addTask', name)
    const result = await axios.post(`/tasks`,{name})   
    return result.data;
  },



  setCompleted: async(id, item) => {
    
    const result = await axios.put(`/tasks/${id}`, item);
    return result.data;
},


  deleteTask:async(id)=>{
    console.log('deleteTask')
    const result = await axios.delete(`/tasks/${id}`)   
    return result.data;
  }
};


/*
אתגר - הזדהות עם JWT
הוסיפי מנגנון הזדהות באמצעות JWT.

הוסיפי טבלת Users עם השדות: מזהה, שם משתמש וסיסמה.
הוסיפי מנגנון הזדהות עם JWT ב-API.
הוסיפי באפליקציית הקליינט דף הרשמה (משתמש חדש) ודף התחברות (לוגין למשתמש קיים)
הוסיפי interceptor ל-axios שתופס את השגיאה 401 ומעביר לדף לוגין.
*/
