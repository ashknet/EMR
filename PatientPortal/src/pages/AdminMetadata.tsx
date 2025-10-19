import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import React, { useState } from 'react';
import { API_BASE } from '../config/apiConfig';

interface MetadataItem { id: number; name: string; isActive: boolean; sortOrder: number; }

const fetchMetadata = async (table: string): Promise<MetadataItem[]> => {
  const res = await fetch(`${API_BASE}/api/metadata/${table}`);
  if (!res.ok) throw new Error('Failed to fetch');
  return res.json();
};

const upsertMetadata = async (table: string, item: MetadataItem) => {
  const res = await fetch(`${API_BASE}/api/metadata/${table}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(item),
  });
  if (!res.ok) throw new Error('Save failed');
};

const deleteMetadata = async (table: string, id: number) => {
  const res = await fetch(`${API_BASE}/api/metadata/${table}/${id}`, { method: 'DELETE' });
  if (!res.ok) throw new Error('Delete failed');
};

export const AdminMetadata: React.FC = () => {
  const [table, setTable] = useState('gender');
  const qc = useQueryClient();
  const { data, isLoading, error } = useQuery(['metadata-admin', table], () => fetchMetadata(table));

  const saveMutation = useMutation((item: MetadataItem) => upsertMetadata(table, item), {
    onSuccess: () => qc.invalidateQueries(['metadata-admin', table]),
  });
  const delMutation = useMutation((id: number) => deleteMetadata(table, id), {
    onSuccess: () => qc.invalidateQueries(['metadata-admin', table]),
  });

  const [newName, setNewName] = useState('');

  return (
    <div className="p-4 max-w-3xl mx-auto">
      <h1 className="text-2xl font-semibold mb-4">Metadata Manager</h1>
      <label className="mb-2 block">
        Select Category
        <select className="select select-bordered" value={table} onChange={e => setTable(e.target.value)}>
          {['gender','maritalstatus','race','language','relationshiptype','insurancetype','smokingstatus','alcoholuse','druguse','livingsituation','communicationpreference'].map(t => (
            <option key={t} value={t}>{t}</option>
          ))}
        </select>
      </label>

      {isLoading && <p>Loading…</p>}
      {error && <p className="text-error">{(error as Error).message}</p>}

      <table className="table w-full">
        <thead>
          <tr><th>Name</th><th>Active</th><th></th></tr>
        </thead>
        <tbody>
          {data?.map(item => (
            <tr key={item.id}>
              <td>{item.name}</td>
              <td>{item.isActive ? 'Yes' : 'No'}</td>
              <td>
                <button className="btn btn-error btn-xs" onClick={() => delMutation.mutate(item.id)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <div className="mt-4 flex gap-2">
        <input className="input input-bordered" placeholder="New name" value={newName} onChange={e => setNewName(e.target.value)} />
        <button className="btn btn-primary" onClick={() => saveMutation.mutate({ id: 0, name: newName, isActive: true, sortOrder: (data?.length || 0) })}>Add</button>
      </div>
    </div>
  );
};

export default AdminMetadata;
