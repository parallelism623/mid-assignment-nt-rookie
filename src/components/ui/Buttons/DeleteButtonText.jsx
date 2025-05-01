import React from "react";
import { Tooltip } from "antd";
import { FiTrash2 } from "react-icons/fi";

const DeleteButtonText = ({ onClick, size }) => {
  return (
    <Tooltip title="Delete">
      <button onClick={onClick} className="p-1 bg-red-300 rounded">
        Delete
      </button>
    </Tooltip>
  );
};
export default DeleteButtonText;
