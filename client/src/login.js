import React, { useState } from 'react';
import service from './service';
import './login.css'; // ייבוא של קובץ העיצוב

function Login({ onLogin }) {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [isRegister, setIsRegister] = useState(false);

    const validateForm = () => {
        if (password.length < 6) {
            alert("הסיסמה חייבת להכיל לפחות 6 תווים");
            return false;
        }
        return true;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        // בדיקת תקינות לפני שליחה
        if (!validateForm()) return;

        try {
            if (isRegister) {
                await service.register(username, password);
                alert("נרשמת בהצלחה! עכשיו אפשר להתחבר");
                setIsRegister(false);
            } else {
                const response = await service.login(username, password);
                localStorage.setItem("userName", username);
                onLogin();
            }
        } catch (error) {
            alert("אופס... משהו השתבש. בדקו את הפרטים");
        }
    };

    return (
        <div className="login-page">
            <div className="login-card">
                <h2>{isRegister ? "יצירת חשבון" : "כניסה למערכת"}</h2>
                <form onSubmit={handleSubmit}>
                    <div className="input-group">
                        <label>שם משתמש</label>
                        <input 
                            type="text" 
                            placeholder="הזן שם משתמש" 
                            value={username} 
                            onChange={(e) => setUsername(e.target.value)} 
                            required 
                        />
                    </div>
                    <div className="input-group">
                        <label>סיסמה</label>
                        <input 
                            type="password" 
                            placeholder="לפחות 6 תווים" 
                            value={password} 
                            onChange={(e) => setPassword(e.target.value)} 
                            required 
                        />
                    </div>
                    <button type="submit" className="submit-btn">
                        {isRegister ? "הירשם עכשיו" : "התחברות"}
                    </button>
                </form>
                <p className="toggle-link" onClick={() => setIsRegister(!isRegister)}>
                    {isRegister ? "כבר רשום? להתחברות" : "משתמש חדש? צור חשבון כאן"}
                </p>
            </div>
        </div>
    );
}

export default Login;


// import React, { useState } from 'react';
// import service from './service';

// function Login({ onLogin }) {
//     const [username, setUsername] = useState("");
//     const [password, setPassword] = useState("");
//     const [isRegister, setIsRegister] = useState(false); // מצב הרשמה או התחברות

//     const handleSubmit = async (e) => {
//         e.preventDefault();
//         try {
//             if (isRegister) {
//                 await service.register(username, password);
//                 alert("נרשמת בהצלחה! עכשיו אפשר להתחבר");
//                 setIsRegister(false);
//             } else {
//                 await service.login(username, password);
//                 onLogin(); // מעדכן את האפליקציה שהתחברנו
//             }
//         } catch (error) {
//             alert("אופס... משהו השתבש. בדקי את הפרטים");
//         }
//     };

//     return (
//         <div className="login-container" style={{ textAlign: 'center', marginTop: '50px' }}>
//             <h2>{isRegister ? "הרשמה למערכת" : "התחברות"}</h2>
//             <form onSubmit={handleSubmit}>
//                 <input 
//                     type="text" 
//                     placeholder="שם משתמש" 
//                     value={username} 
//                     onChange={(e) => setUsername(e.target.value)} 
//                     required 
//                 /><br/>
//                 <input 
//                     type="password" 
//                     placeholder="סיסמה" 
//                     value={password} 
//                     onChange={(e) => setPassword(e.target.value)} 
//                     required 
//                 /><br/><br/>
//                 <button type="submit">{isRegister ? "הירשם" : "התחבר"}</button>
//             </form>
//             <p onClick={() => setIsRegister(!isRegister)} style={{ cursor: 'pointer', color: 'blue' }}>
//                 {isRegister ? "כבר יש לך חשבון? להתחברות" : "משתמש חדש? להרשמה"}
//             </p>
//         </div>
//     );
// }

// export default Login;
