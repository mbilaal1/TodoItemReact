import React, { useEffect, useState } from 'react';
import TodoItem from './TodoItem';
import TodoForm from './TodoForm';

const TodoList = () => {
    const [todoItems, setTodoItems] = useState([]);
    const [includeCompleted, setIncludeCompleted] = useState(false);
    const [newItemText, setNewItemText] = useState('');

    const API_URL = 'https://localhost:7050/api/todo';

    useEffect(() => {
        fetchTodoItems();
    }, [includeCompleted]);

    const fetchTodoItems = async () => {
        const response = await fetch(
            `${API_URL}/list?includeCompleted=${includeCompleted}`
        );
        console.log(response)
        const data = await response.json();
        setTodoItems(data);
    };

    const toggleIncludeCompleted = () => {
        setIncludeCompleted((prevIncludeCompleted) => !prevIncludeCompleted);
    };

    const handleNewItemChange = (e) => {
        setNewItemText(e.target.value);
    };

    const handleNewItemSubmit = async (e) => {
        e.preventDefault();

        const response = await fetch(`${API_URL}/create`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ Text: newItemText }),
        });

        if (response.ok) {
            setNewItemText('');
            fetchTodoItems();
        }
    };

    const handleMostRecentClick = async () => {
        const response = await fetch(`${API_URL}/mostrecentlycreated`);
        console.log(response)
        const data = await response.json();
        setTodoItems(data);
    };

    const markAsCompleted = async (id) => {
        const response = await fetch(`${API_URL}/${id}/completed`, {
            method: 'PUT',
        });

        if (response.ok) {
            fetchTodoItems();
        }
    };

    const formatDateTime = (timestamp) => {
        const options = {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: 'numeric',
            minute: 'numeric',
            second: 'numeric',
        };

        return new Date(timestamp).toLocaleString(undefined, options);
    };

    return (
        <div>
            <h1>Todo List</h1>
            <button onClick={toggleIncludeCompleted} className="btn btn-primary mb-3">
                Toggle Completed
            </button>
            <button onClick={handleMostRecentClick} className="btn btn-primary mb-3 ml-2">
                Most Recent Items
            </button>
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Text</th>
                        <th>Created</th>
                        <th>Completed</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {todoItems.map((item) => (
                        <TodoItem
                            key={item.id}
                            item={item}
                            formatDateTime={formatDateTime}
                            markAsCompleted={markAsCompleted}
                        />
                    ))}
                </tbody>
            </table>
            <TodoForm
                newItemText={newItemText}
                handleNewItemChange={handleNewItemChange}
                handleNewItemSubmit={handleNewItemSubmit}
            />
        </div>
    );
};

export default TodoList;
