import React from "react";
import { Tooltip } from "antd";
import { FiEdit } from "react-icons/fi";

const EditButton = ({ onClick, size }) => (
  <Tooltip title="Edit" key="edit">
    <button onClick={onClick} className="p-1 bg-blue-100 rounded">
      <FiEdit size={size} />
    </button>
  </Tooltip>
);

export default EditButton;
