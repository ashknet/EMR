import { useQuery } from '@tanstack/react-query';
import React, { useState } from 'react';
import { getMetadata, API_BASE } from '../config/apiConfig';

interface SelectOption { id: number; name: string; }

const useMetadata = (table: string) =>
  useQuery<SelectOption[]>({
    queryKey: ['metadata', table],
    queryFn: () => getMetadata(table),
  });

export const PatientIntakeForm: React.FC = () => {
  const { data: genders } = useMetadata('gender');
  const { data: maritalStatuses } = useMetadata('maritalstatus');
  const { data: races } = useMetadata('race');
  const { data: languages } = useMetadata('language');

  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [genderId, setGenderId] = useState<number | undefined>();
  const [homeAddress, setHomeAddress] = useState('');
  const [primaryPhone, setPrimaryPhone] = useState('');
  const [email, setEmail] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const payload = {
      patient: {
        id: 0,
        firstName,
        lastName,
        genderId,
      },
      addresses: [{ addressTypeId: 1, line1: homeAddress }],
      phones: [{ phoneTypeId: 1, phoneNumber: primaryPhone }],
    };
    await fetch(`${API_BASE}/api/patients/intake`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });
    alert('Saved');
  };

  return (
    <form className="space-y-4 p-4 max-w-2xl mx-auto" onSubmit={handleSubmit}>
      <h2 className="text-xl font-semibold">Personal Information</h2>
      <div className="grid grid-cols-2 gap-4">
        <label className="flex flex-col">
          First Name
          <input className="input input-bordered" value={firstName} onChange={e => setFirstName(e.target.value)} />
        </label>
        <label className="flex flex-col">
          Last Name
          <input className="input input-bordered" value={lastName} onChange={e => setLastName(e.target.value)} />
        </label>
        <label className="flex flex-col col-span-2 md:col-span-1">
          Gender
          <select className="select select-bordered" value={genderId ?? ''} onChange={e => setGenderId(Number(e.target.value) || undefined)}>
            <option value="">Select…</option>
            {genders?.map(g => (
              <option key={g.id} value={g.id}>{g.name}</option>
            ))}
          </select>
        </label>
        {/* Additional fields here (DOB, marital status, etc.) */}
      </div>
      <h2 className="text-xl font-semibold mt-8">Contact Information</h2>
      <label className="flex flex-col">
        Home Address
        <textarea className="textarea textarea-bordered" value={homeAddress} onChange={e=>setHomeAddress(e.target.value)} />
      </label>
      <label className="flex flex-col">
        Primary Phone
        <input className="input input-bordered" value={primaryPhone} onChange={e=>setPrimaryPhone(e.target.value)} />
      </label>
      <label className="flex flex-col">
        Email
        <input className="input input-bordered" value={email} onChange={e=>setEmail(e.target.value)} />
      </label>

      <button className="btn btn-primary mt-6" type="submit">Save</button>
    </form>
  );
};

export default PatientIntakeForm;
