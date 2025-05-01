import React from "react";
import { Tooltip } from "antd";
import { FiEye } from "react-icons/fi";

const ViewDetailButton = ({ onClick, size, color }) => {
  return (
    <Tooltip title="Detail" key="view">
      <button onClick={onClick} className="p-1 bg-green-100 rounded">
        <FiEye size={size} color={color} />
      </button>
    </Tooltip>
  );
};

export default ViewDetailButton;
