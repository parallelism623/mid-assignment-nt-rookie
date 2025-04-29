import React from "react";
import { FiStar } from "react-icons/fi";

const FractionalStar = ({ rating, size = 24 }) => {
  const pct = (Math.max(0, Math.min(5, rating)) / 5) * 100;

  return (
    <span
      className="relative inline-block"
      style={{ width: size, height: size }}
    >
      <FiStar size={size} className="text-gray-300" />

      <span
        className="absolute top-0 left-0 overflow-hidden"
        style={{ width: `${pct}%`, height: size }}
      >
        <FiStar size={size} className="text-yellow-400" />
      </span>
    </span>
  );
};

export default FractionalStar;
