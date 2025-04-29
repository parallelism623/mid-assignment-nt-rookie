import React from "react";
import { Tooltip } from "antd";
import { FiTrash2 } from "react-icons/fi";

const DeleteButton = ({ onClick, size }) => {
  return (
    <Tooltip title="Delete">
      <button onClick={onClick} className="p-1 bg-red-100 rounded">
        <FiTrash2 size={size} />
      </button>
    </Tooltip>
  );
};
export default DeleteButton;
