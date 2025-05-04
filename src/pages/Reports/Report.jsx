import { Row, Col } from "antd";
import ReportCard from "./ReportCard";
import { reportType } from "../../constants/reportType";
import { useEffect, useState } from "react";
import dayjs from "dayjs";
import BookBorrowingReport from "./BookBorrowingReport";
import UserEngagementReport from "./UserEngagementReport";
import CategoryReport from "./CategoryReport";
const Report = () => {
  const [option, setOption] = useState(reportType.bookBorrowing);
  const [reportQueryParameters, setReportQueryParameters] = useState({
    ...defaultReportQueryParameters,
  });
  const handleOnReportOptionChange = (values) => {
    console.log(values.option);
    setOption(values.option);
    setReportQueryParameters({ ...reportQueryParameters, ...values });
  };
  return (
    <>
      <Row gutter={[16, 16]}>
        <Col xs={24} md={5}>
          <ReportCard
            onSelect={(values) => {
              handleOnReportOptionChange(values);
            }}
          />
        </Col>
        <Col xs={24} md={19}>
          {option === reportType.bookBorrowing && (
            <BookBorrowingReport
              reportQueryParameters={reportQueryParameters}
            />
          )}
          {option === reportType.category && (
            <CategoryReport reportQueryParameters={reportQueryParameters} />
          )}
          {option === reportType.userEngagement && (
            <UserEngagementReport
              reportQueryParameters={reportQueryParameters}
            />
          )}
        </Col>
      </Row>
    </>
  );
};

export default Report;

const now = dayjs();
const defaultReportQueryParameters = {
  top: 10,
  fromDate: now.subtract(7, "day").format("YYYY-MM-DD"),
  toDate: now.format("YYYY-MM-DD"),
};
