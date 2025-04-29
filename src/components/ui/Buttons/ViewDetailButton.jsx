import React from "react";
import { Tooltip } from "antd";
import { FiEye } from "react-icons/fi";

const ViewDetailButton = () => {
  return (
    <Tooltip title="Detail" key="view">
      <button
        onClick={() => onViewDetail(record)}
        className="p-1 bg-green-100 rounded"
      >
        <FiEye />
      </button>
    </Tooltip>
  );
};

export default ViewDetailButton;
