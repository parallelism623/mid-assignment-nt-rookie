import React from "react";
import { Tooltip } from "antd";
import { FiEdit } from "react-icons/fi";

const EditButtonText = ({ onClick, size }) => (
  <Tooltip title="Edit" key="edit">
    <button size={size} onClick={onClick} className="p-1 bg-blue-300 rounded">
      Edit
    </button>
  </Tooltip>
);

export default EditButtonText;
