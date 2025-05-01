import React from "react";
import { Tooltip } from "antd";

const ReviewButtonText = ({ onClick, size }) => (
  <Tooltip title="Review" arrowPointAtCenter>
    <button
      onClick={onClick}
      size={size}
      className="
        bg-[#f8dc6e] 
        p-1
        rounded
      "
    >
      Review
    </button>
  </Tooltip>
);

export default ReviewButtonText;
