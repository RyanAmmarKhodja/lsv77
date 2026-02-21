import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
    BarChart3, Users, FileText, Car, Wrench, Eye,
    MessageSquare, TrendingUp, Loader2, ShieldAlert, ArrowLeft
} from 'lucide-react';
import api from '../api';
import { useAuth } from '../AuthProvider';

/* ── Color Palette ── */
const CATEGORY_COLORS = {
    COVOITURAGE: { bg: 'bg-blue-500', light: 'bg-blue-100', text: 'text-blue-700' },
    MATERIAL: { bg: 'bg-orange-500', light: 'bg-orange-100', text: 'text-orange-700' },
    MEDIAS: { bg: 'bg-purple-500', light: 'bg-purple-100', text: 'text-purple-700' },
    OUTILS: { bg: 'bg-emerald-500', light: 'bg-emerald-100', text: 'text-emerald-700' },
    AUTRE: { bg: 'bg-gray-500', light: 'bg-gray-100', text: 'text-gray-700' },
};

const CATEGORY_LABELS = {
    COVOITURAGE: 'Covoiturage',
    MATERIAL: 'Matériel',
    MEDIAS: 'Médias',
    OUTILS: 'Outils',
    AUTRE: 'Autre'
};

/* ── Stat Card ── */
const StatCard = ({ icon: Icon, label, value, color = 'orange', sub }) => (
    <div className="bg-white rounded-2xl border border-gray-200 shadow-sm p-5 flex items-start gap-4 hover:shadow-md transition-shadow">
        <div className={`w-12 h-12 rounded-xl flex items-center justify-center flex-shrink-0
            ${color === 'blue' ? 'bg-blue-100 text-blue-600' :
                color === 'green' ? 'bg-emerald-100 text-emerald-600' :
                    color === 'purple' ? 'bg-purple-100 text-purple-600' :
                        color === 'rose' ? 'bg-rose-100 text-rose-600' :
                            'bg-orange-100 text-orange-600'}`}>
            <Icon className="w-6 h-6" />
        </div>
        <div>
            <p className="text-2xl font-bold text-gray-900">{value ?? '—'}</p>
            <p className="text-sm text-gray-500">{label}</p>
            {sub && <p className="text-xs text-gray-400 mt-0.5">{sub}</p>}
        </div>
    </div>
);

/* ── Bar Chart (simple inline SVG) ── */
const SimpleBarChart = ({ data, label }) => {
    if (!data || data.length === 0) return <p className="text-sm text-gray-400 italic">Aucune donnée</p>;
    const maxVal = Math.max(...data.map(d => d.count), 1);
    return (
        <div className="space-y-2">
            <p className="text-sm font-semibold text-gray-600 mb-3">{label}</p>
            {data.map((item, i) => (
                <div key={i} className="flex items-center gap-3">
                    <span className="text-xs text-gray-500 w-24 text-right truncate">{item.date || item.category}</span>
                    <div className="flex-1 bg-gray-100 rounded-full h-5 overflow-hidden">
                        <div
                            className="h-full bg-gradient-to-r from-orange-400 to-orange-500 rounded-full transition-all duration-500"
                            style={{ width: `${(item.count / maxVal) * 100}%`, minWidth: item.count > 0 ? '12px' : '0' }}
                        />
                    </div>
                    <span className="text-xs font-semibold text-gray-600 w-8">{item.count}</span>
                </div>
            ))}
        </div>
    );
};

/* ── Main Page ── */
const Statistics = () => {
    const navigate = useNavigate();
    const { user } = useAuth();
    const [stats, setStats] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (user && user.id !== 13) {
            navigate('/feed', { replace: true });
            return;
        }

        const fetchStats = async () => {
            try {
                const res = await api.get('/stats/dashboard');
                setStats(res.data);
            } catch (err) {
                console.error('Failed to fetch stats:', err);
                if (err.response?.status === 403) {
                    navigate('/feed', { replace: true });
                } else {
                    setError("Impossible de charger les statistiques.");
                }
            } finally {
                setLoading(false);
            }
        };

        if (user) fetchStats();
    }, [user, navigate]);

    if (!user || user.id !== 13) {
        return (
            <div className="min-h-screen flex items-center justify-center bg-gray-50">
                <div className="text-center">
                    <ShieldAlert className="w-16 h-16 text-red-400 mx-auto mb-4" />
                    <p className="text-lg font-semibold text-gray-700">Accès refusé</p>
                    <p className="text-sm text-gray-500 mt-1">Vous n'avez pas la permission d'accéder à cette page.</p>
                </div>
            </div>
        );
    }

    if (loading) {
        return (
            <div className="min-h-screen flex items-center justify-center bg-gray-50">
                <Loader2 className="w-8 h-8 animate-spin text-orange-500" />
            </div>
        );
    }

    if (error) {
        return (
            <div className="min-h-screen flex items-center justify-center bg-gray-50">
                <p className="text-red-500">{error}</p>
            </div>
        );
    }

    // Prepare category chart data
    const categoryData = (stats?.categoryDistribution || []).map(c => ({
        category: CATEGORY_LABELS[c.category] || c.category,
        count: c.count
    }));

    return (
        <div className="min-h-screen bg-gray-50">
            <div className="max-w-6xl mx-auto px-4 py-8">

                {/* Back */}
                <button
                    onClick={() => navigate('/feed')}
                    className="flex items-center gap-1.5 text-sm text-gray-500 hover:text-gray-700 mb-6 transition-colors"
                >
                    <ArrowLeft className="w-4 h-4" />
                    Retour aux annonces
                </button>

                {/* Header */}
                <div className="mb-8">
                    <h1 className="text-3xl font-bold text-gray-900 flex items-center gap-3">
                        <BarChart3 className="w-8 h-8 text-orange-500" />
                        Statistiques
                    </h1>
                    <p className="text-sm text-gray-500 mt-1">Tableau de bord administrateur</p>
                </div>

                {/* KPI Cards */}
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
                    <StatCard icon={Users} label="Utilisateurs" value={stats?.totalUsers} color="blue" />
                    <StatCard icon={FileText} label="Annonces" value={stats?.totalPosts} color="orange"
                        sub={`${stats?.activePosts || 0} actives`} />
                    <StatCard icon={Car} label="Trajets confirmés" value={stats?.totalRidesConfirmed} color="green"
                        sub="Clics 'Envoyer un message'" />
                    <StatCard icon={Wrench} label="Matériel prêté" value={stats?.totalEquipmentLoaned} color="purple"
                        sub="Clics 'Envoyer un message'" />
                </div>

                {/* Second row */}
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 mb-8">
                    <StatCard icon={MessageSquare} label="Conversations" value={stats?.totalConversations} color="rose" />
                    <StatCard icon={MessageSquare} label="Messages envoyés" value={stats?.totalMessages} color="blue" />
                    <StatCard icon={TrendingUp} label="Posts les + vus" value={stats?.mostViewedPosts?.[0]?.viewCount || 0}
                        color="green" sub={stats?.mostViewedPosts?.[0]?.title || '—'} />
                </div>

                {/* Charts */}
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
                    {/* Category distribution */}
                    <div className="bg-white rounded-2xl border border-gray-200 shadow-sm p-6">
                        <h2 className="text-lg font-bold text-gray-900 mb-4">Répartition des catégories</h2>
                        <div className="space-y-3">
                            {categoryData.map((cat, i) => {
                                const key = stats?.categoryDistribution?.[i]?.category;
                                const colors = CATEGORY_COLORS[key] || CATEGORY_COLORS.AUTRE;
                                const total = categoryData.reduce((s, c) => s + c.count, 0) || 1;
                                const pct = ((cat.count / total) * 100).toFixed(1);
                                return (
                                    <div key={i} className="flex items-center gap-3">
                                        <span className={`inline-block w-3 h-3 rounded-full ${colors.bg}`} />
                                        <span className="text-sm text-gray-700 w-24">{cat.category}</span>
                                        <div className="flex-1 bg-gray-100 rounded-full h-4 overflow-hidden">
                                            <div className={`h-full rounded-full ${colors.bg} transition-all duration-500`}
                                                style={{ width: `${pct}%`, minWidth: cat.count > 0 ? '8px' : '0' }} />
                                        </div>
                                        <span className="text-xs font-semibold text-gray-600 w-16 text-right">
                                            {cat.count} ({pct}%)
                                        </span>
                                    </div>
                                );
                            })}
                        </div>
                    </div>

                    {/* Posts per day */}
                    <div className="bg-white rounded-2xl border border-gray-200 shadow-sm p-6">
                        <h2 className="text-lg font-bold text-gray-900 mb-4">Annonces par jour (30 derniers jours)</h2>
                        <SimpleBarChart data={stats?.postsPerDay} label="" />
                    </div>
                </div>

                {/* Most viewed posts table */}
                <div className="bg-white rounded-2xl border border-gray-200 shadow-sm p-6">
                    <h2 className="text-lg font-bold text-gray-900 mb-4 flex items-center gap-2">
                        <Eye className="w-5 h-5 text-orange-500" />
                        Annonces les plus consultées
                    </h2>
                    {stats?.mostViewedPosts?.length > 0 ? (
                        <div className="overflow-x-auto">
                            <table className="w-full text-sm">
                                <thead>
                                    <tr className="border-b border-gray-200">
                                        <th className="text-left py-3 px-2 text-gray-500 font-medium">#</th>
                                        <th className="text-left py-3 px-2 text-gray-500 font-medium">Titre</th>
                                        <th className="text-left py-3 px-2 text-gray-500 font-medium">Catégorie</th>
                                        <th className="text-left py-3 px-2 text-gray-500 font-medium">Auteur</th>
                                        <th className="text-right py-3 px-2 text-gray-500 font-medium">Vues</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {stats.mostViewedPosts.map((post, i) => {
                                        const colors = CATEGORY_COLORS[post.category] || CATEGORY_COLORS.AUTRE;
                                        return (
                                            <tr key={post.id} className="border-b border-gray-100 hover:bg-gray-50 cursor-pointer transition-colors"
                                                onClick={() => navigate(`/post/${post.id}`)}>
                                                <td className="py-3 px-2 text-gray-400 font-mono">{i + 1}</td>
                                                <td className="py-3 px-2 font-medium text-gray-900 max-w-[200px] truncate">{post.title}</td>
                                                <td className="py-3 px-2">
                                                    <span className={`px-2 py-1 rounded-full text-xs font-medium ${colors.light} ${colors.text}`}>
                                                        {CATEGORY_LABELS[post.category] || post.category}
                                                    </span>
                                                </td>
                                                <td className="py-3 px-2 text-gray-600">{post.authorName}</td>
                                                <td className="py-3 px-2 text-right font-bold text-orange-600">{post.viewCount}</td>
                                            </tr>
                                        );
                                    })}
                                </tbody>
                            </table>
                        </div>
                    ) : (
                        <p className="text-sm text-gray-400 italic">Aucune donnée de consultation pour le moment.</p>
                    )}
                </div>
            </div>
        </div>
    );
};

export default Statistics;
