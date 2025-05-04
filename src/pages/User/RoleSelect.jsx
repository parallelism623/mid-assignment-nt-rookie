import { Select } from "antd";
import { useEffect, useState } from "react";
import { roleServices } from "../../services/roleServices";
import { FiTag } from "react-icons/fi";
const RoleSelect = ({ onSelect }) => {
  const [loading, setLoading] = useState(true);
  const [roles, setRoles] = useState([]);
  useEffect(() => {
    setLoading(true);
    roleServices
      .get()
      .then((res) => {
        setRoles([...res]);
        setLoading(false);
      })
      .catch(() => {
        setLoading(false);
      });
  }, []);
  return (
    <Select
      placeholder="Select roles"
      loading={loading}
      mode="multiple"
      className="rounded-lg !min-w-40 !w-fit"
      suffixIcon={<FiTag />}
      onChange={(value) => {
        onSelect(value);
      }}
    >
      {roles.map((c) => (
        <Option key={c.id} value={c.id}>
          {c.name}
        </Option>
      ))}
    </Select>
  );
};

export default RoleSelect;
