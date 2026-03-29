import React, { useEffect, useState } from 'react';
import { useSearchParams, Link } from 'react-router-dom';
import api from '../api';

const VerifyEmail = () => {
    const [searchParams] = useSearchParams();
    const [status, setStatus] = useState('loading'); // 'loading', 'success', 'error'
    const [message, setMessage] = useState('Verifying your account...');

    useEffect(() => {
        const verifyToken = async () => {
            const token = searchParams.get('token');

            if (!token) {
                setStatus('error');
                setMessage('Missing verification token.');
                return;
            }

            try {
                // Adjust this URL to your actual .NET backend address
                const response = await api.get(`/users/verify-email?token=${token}`);
                setStatus('success');
                setMessage(response.data.message || 'Email verified successfully!');
            } catch (err) {
                setStatus('error');
                setMessage(err.response?.data?.message || 'Verification failed. Link may be expired.');
            }
        };

        verifyToken();
    }, [searchParams]);

    return (
        <div style={styles.container}>
            <div style={styles.card}>
                <h2>Email Verification</h2>
                <hr />
                <p style={{ color: status === 'error' ? 'red' : 'black' }}>{message}</p>
                
                {status !== 'loading' && (
                    <Link style={styles.button} to="/login">
                        Go to Login
                    </Link>
                )}
            </div>
        </div>
    );
};

const styles = {
    container: { display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh', backgroundColor: '#f4f7f6' },
    card: { padding: '2rem', borderRadius: '8px', backgroundColor: '#fff', boxShadow: '0 4px 6px rgba(0,0,0,0.1)', textAlign: 'center', minWidth: '300px' },
    button: { display: 'inline-block', marginTop: '1rem', padding: '10px 20px', backgroundColor: '#007bff', color: '#fff', textDecoration: 'none', borderRadius: '4px' }
};

export default VerifyEmail;