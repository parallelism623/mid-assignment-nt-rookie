import React from "react";
import { Result, Button } from "antd";
import { Link } from "react-router";
const ForbiddenPage = () => {
  return (
    <Result
      status="403"
      title={<span className="text-[#EBECF0]">403</span>}
      subTitle={
        <>
          <span className="text-[#EBECF0]">
            "Sorry, you are not authorized to access this page."
          </span>
        </>
      }
      extra={
        <Button type="primary">
          <Link to="/">Back Home</Link>
        </Button>
      }
    />
  );
};

export default ForbiddenPage;
