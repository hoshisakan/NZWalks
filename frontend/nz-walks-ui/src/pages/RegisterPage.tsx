import { useState } from 'react';
import type { FormEvent } from 'react';
import axios from 'axios';
import { Link, useNavigate } from 'react-router-dom';
import { useToast } from '../components/ui/ToastProvider';
import apiClient from '../services/apiClient';

const RegisterPage = () => {
    const navigate = useNavigate();
    const { pushToast } = useToast();
    const [formData, setFormData] = useState({
        Username: '',
        Email: '',
        Password: '',
        ConfirmPassword: '',
        Roles: ['Reader'],
    });
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState('');

    const toggleRole = (role: 'Reader' | 'Writer') => {
        setFormData((prev) => {
            const exists = prev.Roles.includes(role);
            return {
                ...prev,
                Roles: exists ? prev.Roles.filter((item) => item !== role) : [...prev.Roles, role],
            };
        });
    };

    const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        setError('');

        if (formData.Password !== formData.ConfirmPassword) {
            const message = 'Password and confirm password do not match.';
            setError(message);
            pushToast(message, 'error');
            return;
        }

        if (formData.Roles.length === 0) {
            const message = 'Please select at least one role.';
            setError(message);
            pushToast(message, 'error');
            return;
        }

        setSubmitting(true);

        try {
            await apiClient.post('/Auth/Register', {
                Username: formData.Username.trim(),
                Email: formData.Email.trim(),
                Password: formData.Password,
                Roles: formData.Roles,
            });

            pushToast('Register successful. Please login.', 'success');
            navigate('/');
        } catch (err) {
            const message = axios.isAxiosError(err)
                ? err.response?.data?.title || err.response?.data?.message || err.response?.data || err.message
                : err instanceof Error
                  ? err.message
                  : 'Register failed.';
            setError(message);
            pushToast(message, 'error');
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <main className="min-h-screen bg-slate-50 p-6 md:p-10">
            <div className="mx-auto w-full max-w-xl rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
                <h1 className="text-2xl font-bold text-slate-800">Create Account</h1>
                <p className="mt-1 text-sm text-slate-500">
                    Register API:{' '}
                    <code className="rounded bg-slate-100 px-1 py-0.5">POST https://localhost/api/Auth/Register</code>
                </p>

                <form className="mt-5 grid gap-3" onSubmit={handleSubmit}>
                    <input
                        className="rounded-md border border-slate-300 px-3 py-2 text-sm"
                        value={formData.Username}
                        onChange={(event) => setFormData((prev) => ({ ...prev, Username: event.target.value }))}
                        placeholder="Username"
                        required
                    />
                    <input
                        className="rounded-md border border-slate-300 px-3 py-2 text-sm"
                        type="email"
                        value={formData.Email}
                        onChange={(event) => setFormData((prev) => ({ ...prev, Email: event.target.value }))}
                        placeholder="Email"
                        required
                    />
                    <input
                        className="rounded-md border border-slate-300 px-3 py-2 text-sm"
                        type="password"
                        value={formData.Password}
                        onChange={(event) => setFormData((prev) => ({ ...prev, Password: event.target.value }))}
                        placeholder="Password"
                        required
                    />
                    <input
                        className="rounded-md border border-slate-300 px-3 py-2 text-sm"
                        type="password"
                        value={formData.ConfirmPassword}
                        onChange={(event) => setFormData((prev) => ({ ...prev, ConfirmPassword: event.target.value }))}
                        placeholder="Confirm password"
                        required
                    />

                    <div className="rounded-md border border-slate-200 p-3">
                        <p className="mb-2 text-sm font-medium text-slate-700">Roles</p>
                        <div className="flex gap-4">
                            <label className="flex items-center gap-2 text-sm text-slate-700">
                                <input
                                    type="checkbox"
                                    checked={formData.Roles.includes('Reader')}
                                    onChange={() => toggleRole('Reader')}
                                />
                                Reader
                            </label>
                            <label className="flex items-center gap-2 text-sm text-slate-700">
                                <input
                                    type="checkbox"
                                    checked={formData.Roles.includes('Writer')}
                                    onChange={() => toggleRole('Writer')}
                                />
                                Writer
                            </label>
                        </div>
                    </div>

                    <button
                        type="submit"
                        className="rounded-md bg-blue-600 px-3 py-2 text-sm font-semibold text-white hover:bg-blue-500 disabled:opacity-70"
                        disabled={submitting}
                    >
                        {submitting ? 'Registering...' : 'Register'}
                    </button>
                </form>

                {error && (
                    <p className="mt-3 rounded-md border border-rose-200 bg-rose-50 px-3 py-2 text-sm text-rose-700">
                        {error}
                    </p>
                )}

                <p className="mt-4 text-sm text-slate-600">
                    Already have an account?{' '}
                    <Link className="font-semibold text-blue-600 hover:text-blue-500" to="/">
                        Back to dashboard login
                    </Link>
                </p>
            </div>
        </main>
    );
};

export default RegisterPage;
