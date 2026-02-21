import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import {
    MapPin, Clock, Users, ArrowRight, ArrowLeft, Car, Wrench,
    Loader2, Trash2, User as UserIcon, Mail, Calendar, Tag, Eye, EyeOff,
    Film, Cpu, HelpCircle, MessageSquare
} from 'lucide-react';
import api from '../api';
import { useAuth } from '../AuthProvider';

/* ── Helpers ── */

function formatDate(dateStr) {
    return new Date(dateStr).toLocaleDateString('fr-FR', {
        day: 'numeric',
        month: 'long',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
    });
}

function timeAgo(dateStr) {
    const now = new Date();
    const date = new Date(dateStr);
    const diffMs = now - date;
    const diffMin = Math.floor(diffMs / 60000);
    if (diffMin < 1) return "À l'instant";
    if (diffMin < 60) return `Il y a ${diffMin} min`;
    const diffH = Math.floor(diffMin / 60);
    if (diffH < 24) return `Il y a ${diffH}h`;
    const diffD = Math.floor(diffH / 24);
    if (diffD < 30) return `Il y a ${diffD}j`;
    return date.toLocaleDateString('fr-FR');
}

const CATEGORY_CONFIG = {
    COVOITURAGE: { label: 'Covoiturage', icon: Car, color: 'blue' },
    MATERIAL: { label: 'Matériel', icon: Wrench, color: 'orange' },
    MEDIAS: { label: 'Médias', icon: Film, color: 'purple' },
    OUTILS: { label: 'Outils', icon: Cpu, color: 'emerald' },
    AUTRE: { label: 'Autre', icon: HelpCircle, color: 'gray' },
};

const TYPE_BADGE = {
    OFFER: { label: 'Proposition', bg: 'bg-green-100', text: 'text-green-700', border: 'border-green-200' },
    DEMAND: { label: 'Recherche', bg: 'bg-red-100', text: 'text-red-700', border: 'border-red-200' },
};

/* ── Post Detail Page ── */

const Post = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const { user } = useAuth();
    const [post, setPost] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [deleting, setDeleting] = useState(false);

    useEffect(() => {
        const fetchPost = async () => {
            setLoading(true);
            setError(null);
            try {
                const res = await api.get(`/post/${id}`);
                setPost(res.data);
            } catch (err) {
                console.error('Failed to fetch post:', err);
                setError("Impossible de charger l'annonce.");
            } finally {
                setLoading(false);
            }
        };
        fetchPost();
    }, [id]);

    const handleDeactivate = async () => {
        if (!window.confirm('Voulez-vous vraiment désactiver cette annonce ?')) return;
        setDeleting(true);
        try {
            await api.patch(`/post/${post.id}/deactivate`);
            navigate('/feed');
        } catch (err) {
            console.error('Failed to deactivate post:', err);
            alert("Échec de la désactivation.");
            setDeleting(false);
        }
    };

    if (loading) {
        return (
            <div className="min-h-screen bg-gray-50 flex items-center justify-center">
                <Loader2 className="w-10 h-10 text-orange-500 animate-spin" />
            </div>
        );
    }

    if (error || !post) {
        return (
            <div className="min-h-screen bg-gray-50 flex flex-col items-center justify-center gap-4">
                <p className="text-red-500 text-lg">{error || "Annonce introuvable."}</p>
                <button
                    onClick={() => navigate('/feed')}
                    className="text-orange-600 hover:text-orange-700 font-medium flex items-center gap-1"
                >
                    <ArrowLeft className="w-4 h-4" /> Retour au fil
                </button>
            </div>
        );
    }

    const catKey = (post.category || 'AUTRE').toUpperCase();
    const catConfig = CATEGORY_CONFIG[catKey] || CATEGORY_CONFIG.AUTRE;
    const CatIcon = catConfig.icon;
    const typeKey = (post.postType || 'OFFER').toUpperCase();
    const typeBadge = TYPE_BADGE[typeKey] || TYPE_BADGE.OFFER;
    const isCovoiturage = catKey === 'COVOITURAGE';
    const isAuthor = user && (user.id === post.authorId || user.id === post.author?.id);

    return (
        <div className="min-h-screen bg-gray-50">
            <div className="max-w-3xl mx-auto px-4 py-8">

                {/* ── Back Button ── */}
                <button
                    onClick={() => navigate('/feed')}
                    className="flex items-center gap-1.5 text-sm text-gray-500 hover:text-gray-700 mb-6 transition-colors"
                >
                    <ArrowLeft className="w-4 h-4" />
                    Retour aux annonces
                </button>

                {/* ── Main Card ── */}
                <div className="bg-white rounded-2xl border border-gray-200 shadow-sm overflow-hidden">

                    {/* Header gradient bar */}
                    <div className={`px-6 py-4 bg-gradient-to-r ${catConfig.color === 'blue' ? 'from-blue-500 to-blue-600' :
                        catConfig.color === 'orange' ? 'from-orange-500 to-orange-600' :
                            catConfig.color === 'purple' ? 'from-purple-500 to-purple-600' :
                                catConfig.color === 'emerald' ? 'from-emerald-500 to-emerald-600' :
                                    'from-gray-500 to-gray-600'
                        }`}
                    >
                        <div className="flex items-center justify-between">
                            <span className="text-white text-sm font-semibold flex items-center gap-2">
                                <CatIcon className="w-4 h-4" />
                                {catConfig.label}
                            </span>
                            <span className="text-white/70 text-xs">{timeAgo(post.createdAt)}</span>
                        </div>
                    </div>

                    {/* Image */}
                    {post.imageUrl && (
                        <div className="h-64 overflow-hidden">
                            <img
                                src={post.imageUrl}
                                alt={post.title}
                                className="w-full h-full object-cover"
                            />
                        </div>
                    )}

                    {/* Body */}
                    <div className="p-6">

                        {/* Badges row */}
                        <div className="flex flex-wrap items-center gap-2 mb-4">
                            <span className={`px-3 py-1 rounded-full text-xs font-semibold border ${typeBadge.bg} ${typeBadge.text} ${typeBadge.border}`}>
                                {typeBadge.label}
                            </span>
                            <span className="px-3 py-1 rounded-full text-xs font-medium bg-gray-100 text-gray-600 border border-gray-200">
                                <Tag className="w-3 h-3 inline mr-1" />
                                {catConfig.label}
                            </span>
                            {!post.isActive && (
                                <span className="px-3 py-1 rounded-full text-xs font-semibold bg-yellow-100 text-yellow-700 border border-yellow-200 flex items-center gap-1">
                                    <EyeOff className="w-3 h-3" /> Désactivée
                                </span>
                            )}
                        </div>

                        {/* Title */}
                        <h1 className="text-2xl font-bold text-gray-900 mb-4">{post.title}</h1>

                        {/* Covoiturage route info */}
                        {isCovoiturage && (
                            <div className="bg-blue-50 rounded-xl p-5 mb-5 border border-blue-100">
                                <div className="flex items-center gap-4">
                                    <div className="flex-1 text-center">
                                        <p className="text-xs text-blue-500 font-medium mb-1">Départ</p>
                                        <p className="font-bold text-gray-900">{post.departureLocation}</p>
                                    </div>
                                    <div className="flex-shrink-0">
                                        <ArrowRight className="w-6 h-6 text-blue-400" />
                                    </div>
                                    <div className="flex-1 text-center">
                                        <p className="text-xs text-blue-500 font-medium mb-1">Destination</p>
                                        <p className="font-bold text-gray-900">{post.destinationLocation}</p>
                                    </div>
                                </div>

                                <div className="flex flex-wrap items-center gap-4 mt-4 pt-4 border-t border-blue-100">
                                    {post.departureTime && (
                                        <span className="flex items-center gap-1.5 text-sm text-gray-600">
                                            <Clock className="w-4 h-4 text-blue-400" />
                                            Départ : {formatDate(post.departureTime)}
                                        </span>
                                    )}
                                    {post.availableSeats != null && (
                                        <span className="flex items-center gap-1.5 text-sm text-gray-600">
                                            <Users className="w-4 h-4 text-blue-400" />
                                            {post.availableSeats} place{post.availableSeats > 1 ? 's' : ''}
                                        </span>
                                    )}
                                    {post.returnTime && (
                                        <span className="flex items-center gap-1.5 text-sm text-gray-600">
                                            <Clock className="w-4 h-4 text-blue-400" />
                                            Retour : {formatDate(post.returnTime)}
                                        </span>
                                    )}
                                </div>
                            </div>
                        )}

                        {/* Location (equipment) */}
                        {!isCovoiturage && post.location && (
                            <div className="flex items-center gap-2 text-sm text-gray-600 mb-4">
                                <MapPin className="w-4 h-4 text-gray-400" />
                                {post.location}
                            </div>
                        )}

                        {/* Description */}
                        <div className="mb-6">
                            <h2 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-2">Description</h2>
                            <p className="text-gray-700 leading-relaxed whitespace-pre-wrap">{post.content}</p>
                        </div>

                        {/* Metadata */}
                        <div className="flex items-center gap-4 text-xs text-gray-400 mb-6">
                            <span className="flex items-center gap-1">
                                <Calendar className="w-3.5 h-3.5" />
                                Publié le {formatDate(post.createdAt)}
                            </span>
                            {post.viewCount > 0 && (
                                <span className="flex items-center gap-1">
                                    <Eye className="w-3.5 h-3.5" />
                                    {post.viewCount} vue{post.viewCount > 1 ? 's' : ''}
                                </span>
                            )}
                        </div>

                        {/* Author actions */}
                        {isAuthor && (
                            <div className="pt-4 border-t border-gray-100">
                                <button
                                    onClick={handleDeactivate}
                                    disabled={deleting}
                                    className="flex items-center gap-2 px-4 py-2.5 rounded-lg bg-red-50 text-red-600 hover:bg-red-100 border border-red-200 text-sm font-medium transition-colors disabled:opacity-50"
                                >
                                    {deleting ? (
                                        <Loader2 className="w-4 h-4 animate-spin" />
                                    ) : (
                                        <Trash2 className="w-4 h-4" />
                                    )}
                                    Désactiver l'annonce
                                </button>
                            </div>
                        )}
                    </div>
                </div>

                {/* ── Author Card ── */}
                {post.author && (
                    <div className="mt-6 bg-white rounded-2xl border border-gray-200 shadow-sm p-6">
                        <h2 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-4">
                            {typeKey === 'OFFER' ? 'Proposé par' : 'Recherché par'}
                        </h2>
                        <div className="flex items-center gap-4">
                            <div className="w-14 h-14 rounded-full bg-gradient-to-br from-orange-400 to-orange-600 flex items-center justify-center text-white text-xl font-bold shadow-sm">
                                {(post.author.firstName || '?')[0]}
                            </div>
                            <div className="flex-1">
                                <p className="font-bold text-gray-900 text-lg">
                                    {post.author.firstName} {post.author.lastName}
                                </p>
                                <p className="text-sm text-gray-500 flex items-center gap-1.5 mt-0.5">
                                    <Mail className="w-3.5 h-3.5" />
                                    {post.author.email}
                                </p>
                            </div>
                        </div>
                        {/* Send message button (hidden for own posts) */}
                        {!isAuthor && (
                            <button
                                onClick={() => navigate(`/chat?userId=${post.author.id}&postId=${post.id}`)}
                                className="mt-4 w-full flex items-center justify-center gap-2 px-4 py-3 rounded-xl bg-[#F56B2A] text-white hover:bg-[#E35B1D] font-semibold text-sm transition-colors shadow-sm"
                            >
                                <MessageSquare className="w-4 h-4" />
                                Envoyer un message
                            </button>
                        )}
                    </div>
                )}
            </div>
        </div>
    );
};

export default Post;
