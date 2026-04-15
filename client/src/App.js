import React, { useEffect, useState } from 'react';
import service from './service.js';
import Login from './login.js';


function App() {
  const [newTodo, setNewTodo] = useState("");
  const [todos, setTodos] = useState([]);
  const [isLoggedIn, setIsLoggedIn] = useState(!!localStorage.getItem("token"));
  const [userName, setUserName] = useState("");

  async function getTodos() {
    if(isLoggedIn){
    const todos = await service.getTasks();
    setTodos(todos);
    }
  }

  // --- פונקציית התנתקות ---
  function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("userName"); 
    setIsLoggedIn(false);
    setUserName("");
  }

  async function createTodo(e) {
    e.preventDefault();
    await service.addTask(newTodo);
    setNewTodo("");//clear input
    await getTodos();//refresh tasks list (in order to see the new one)
  }

  async function updateCompleted(todo, isComplete) {    
    todo.isComplete=isComplete
    await service.setCompleted(todo.id, todo);
    await getTodos();//refresh tasks list (in order to see the updated one)
  }

  async function deleteTodo(id) {
    await service.deleteTask(id);
    await getTodos();//refresh tasks list
  }

  useEffect(() => {
    if (isLoggedIn) {
      const savedName = localStorage.getItem("userName");
      if (savedName) {
        setUserName(savedName);
      }
    }
    getTodos();
  }, [isLoggedIn]);

  if(!isLoggedIn){
    return <Login onLogin={()=>setIsLoggedIn(true)}/>;
  }

  return (
    <section className="todoapp">
      <p style={{ fontSize: '18px', color: '#666', textAlign: 'right', padding: '0 15px' }}>
           ! שלום ל{userName || 'אורח'}
        </p> 
      <h1>todos</h1>
      <header className="header">
      

        <button onClick={logout} style={{float: 'left', margin: '10px'}}>התנתק</button>
       
        <form onSubmit={createTodo}>
          <input className="new-todo" placeholder="Well, let's take on the day" value={newTodo} onChange={(e) => setNewTodo(e.target.value)} />
        </form>
      </header>
      <section className="main" style={{ display: "block" }}>
        <ul className="todo-list">
          {todos.map(todo => {
            return (
              <li className={todo.isComplete ? "completed" : ""} key={todo.id}>
                <div className="view">
                  <input className="toggle" type="checkbox" defaultChecked={todo.isComplete} onChange={(e) => updateCompleted(todo, e.target.checked)} />
                  <label>{todo.name}</label>
                  <button className="destroy" onClick={() => deleteTodo(todo.id)}></button>
                </div>
              </li>
            );
          })}
        </ul>
      </section>
    </section >
  );
}

export default App;