import { Spin } from "antd";
import { FaBook } from "react-icons/fa";
import { LoadingOutlined } from "@ant-design/icons";
const LoadingPage = () => {
  return (
    <div className="loading-page flex flex-col justify-center items-center h-screen w-screen bg-[#1D2027]">
      <FaBook className="text-[4rem] text-[var(--font-color-dark-mode)] mb-6    " />
      <Spin
        indicator={
          <LoadingOutlined
            style={{ fontSize: 24, color: "var(--font-color-dark-mode)" }}
            spin
          />
        }
      />
    </div>
  );
};

export default LoadingPage;
