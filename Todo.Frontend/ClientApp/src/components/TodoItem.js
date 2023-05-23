import React from 'react';

const TodoItem = ({ item, formatDateTime, markAsCompleted }) => {
    return (
        <tr key={item.id}>
            <td>{item.id}</td>
            <td>{item.text}</td>
            <td>{formatDateTime(item.created)}</td>
            <td>{item.completed ? formatDateTime(item.completed) : '-'}</td>
            <td>
                {!item.completed && (
                    <button onClick={() => markAsCompleted(item.id)} className="btn btn-success btn-sm">
                        Mark as Completed
                    </button>
                )}
            </td>
        </tr>
    );
};

export default TodoItem;
