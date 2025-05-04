import React, { useEffect, useState } from "react";
import { List, Card, Pagination } from "antd";
import { auditLogServices } from "../../services/auditLogServices";
import { useUserContext } from "../../routes/ProtectedRoute";
import { environment } from "../../constants/environment";
const activityData = [
  { item: "Authentication", label: "Authentication" },
  {
    item: "BookBorrowingRequestDetail",
    label: "Book borrowing request details",
  },
  { item: "BookBorrowingRequest", label: "Book borrowing requests" },
  { item: "BookReview", label: "Book reviews" },
];

const activityDataAdmin = [
  { item: "Book", label: "Books" },
  { item: "Category", label: "Categories" },
  { item: "Role", label: "Roles" },
  { item: "User", label: "Users" },
];
const defaultQueryParameter = {
  pageIndex: 1,
  pageSize: 10,
  entityName: "",
  totalCount: 0,
};
const ActivityLogPanel = () => {
  const { id, roleName } = useUserContext();
  const [selected, setSelected] = useState(null);
  const [queryParameters, setQueryParameters] = useState({
    ...defaultQueryParameter,
  });
  let activityDataOptions = activityData;
  if (roleName === environment.adminRole)
    activityDataOptions = activityDataOptions.concat(activityDataAdmin);
  const [paginatedData, setPaginatedData] = useState([]);
  const handleSelect = (it) => {
    if (selected === it) {
      setSelected(null);
      setPaginatedData([]);
    } else {
      setPaginatedData([]);
      setSelected(it);
      setQueryParameters({ ...defaultQueryParameter, entityName: it });
    }
  };
  const setPage = (pageIndex, pageSize) => {
    setQueryParameters({ ...queryParameters, pageIndex, pageSize });
  };
  useEffect(() => {
    auditLogServices.getUserAuditLog(id, queryParameters).then((res) => {
      setPaginatedData(res.items);
      setQueryParameters({ ...queryParameters, totalCount: res.totalCount });
    });
  }, [selected, queryParameters.pageIndex, queryParameters.pageSize]);
  return (
    <Card
      title="Activity Log"
      className="rounded-xl shadow border bg-white w-full"
      headStyle={{ fontWeight: "bold", fontSize: "16px" }}
    >
      <List
        dataSource={activityDataOptions}
        renderItem={({ item, label }) => (
          <>
            <List.Item
              className={`cursor-pointer px-3 py-2 transition rounded-md hover:bg-gray-100 ${
                selected === item
                  ? "bg-gray-100 font-semibold text-blue-600"
                  : ""
              }`}
              onClick={() => handleSelect(item)}
            >
              {label}
            </List.Item>

            {selected === item && (
              <div className="px-3 pb-4">
                <List
                  className="!mt-5 whitespace-pre-line"
                  bordered
                  size="small"
                  dataSource={paginatedData}
                  renderItem={(log) => (
                    <List.Item>
                      {log.description.replace(/\|\|/g, "\n")}
                    </List.Item>
                  )}
                  style={{ maxHeight: "300px", overflowY: "auto" }}
                />
                <div className="flex justify-end mt-2">
                  <Pagination
                    size="small"
                    current={queryParameters.pageIndex}
                    total={queryParameters.totalCount}
                    pageSize={queryParameters.pageSize}
                    onChange={(pageIndex, pageSize) =>
                      setPage(pageIndex, pageSize)
                    }
                  />
                </div>
              </div>
            )}
          </>
        )}
      />
    </Card>
  );
};

export default ActivityLogPanel;
