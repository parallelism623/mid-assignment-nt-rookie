import React from "react";
import { Tooltip } from "antd";
import { FiStar } from "react-icons/fi";

const ReviewButton = ({ onClick, size }) => (
  <Tooltip title="Review" arrowPointAtCenter>
    <button
      onClick={onClick}
      className="
        bg-[#F9E79F] 
        p-1
        rounded
      "
    >
      <FiStar size={size} />
    </button>
  </Tooltip>
);

export default ReviewButton;
