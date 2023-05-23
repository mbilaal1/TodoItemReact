import React from 'react';

const TodoForm = ({ newItemText, handleNewItemChange, handleNewItemSubmit }) => {
    return (
        <div className="mt-4">
            <h2>Create New Item</h2>
            <form onSubmit={handleNewItemSubmit}>
                <div className="input-group">
                    <input
                        type="text"
                        className="form-control"
                        placeholder="Enter text"
                        value={newItemText}
                        onChange={handleNewItemChange}
                    />
                    <button type="submit" className="btn btn-primary">
                        Create Item
                    </button>
                </div>
            </form>
        </div>
    );
};

export default TodoForm;
